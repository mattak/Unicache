using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unicache.Plugin;
using UniRx;
using UnityEngine;

namespace Unicache
{
    public class FileCache : IUnicache
    {
        public class Command
        {
            public string Key;
            public string Path;
            public string Url;
            public byte[] Data;

            public Command(string key, string path, string url)
            {
                this.Key = key;
                this.Path = path;
                this.Url = url;
            }

            public Command SetData(byte[] data)
            {
                this.Data = data;
                return this;
            }
        }

        public ICacheHandler Handler { get; set; }
        public IUrlLocator UrlLocator { get; set; }
        public ICacheLocator CacheLocator { get; set; }
        public ICacheEncoder Encoder { get; set; }
        public ICacheDecoder Decoder { get; set; }
        private string RootDirectory;

        private ISubject<Command> RequestQueue = new Subject<Command>();
        private ISubject<Command> ResultQueue = new Subject<Command>();

        public FileCache(GameObject gameObject) : this(gameObject, UnicacheConfig.Directory,
            new VoidEncoderDecoder())
        {
        }

        public FileCache(GameObject gameObject, string rootDirectory, ICacheEncoderDecoder encoderDecoder)
        {
            this.RootDirectory = rootDirectory;
            this.Encoder = encoderDecoder;
            this.Decoder = encoderDecoder;
            this.BuildQueue(gameObject);
        }

        public IObservable<byte[]> Fetch(string key)
        {
            var url = this.UrlLocator.CreateUrl(key);
            var path = this.CacheLocator.CreateCachePath(key);

            this.RequestQueue.OnNext(new Command(key, path, url));

            return this.ResultQueue
                .Where(command => command.Key.Equals(key))
                .Select(command => command.Data)
                .First();
        }

        public void Clear()
        {
            try
            {
                IO.CleanDirectory(this.RootDirectory);
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        public void Delete(string key)
        {
            try
            {
                var allPathes = ListPathes();
                var keyPathes = new List<string>(this.CacheLocator.GetSameKeyCachePathes(key, allPathes));

                foreach (var path in keyPathes)
                {
                    this.DeleteByPath(path);
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        public void DeleteByPath(string path)
        {
            File.Delete(this.RootDirectory + path);
        }

        public byte[] GetCache(string key)
        {
            return this.GetCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        protected byte[] GetCacheByPath(string path)
        {
            var data = IO.Read(this.RootDirectory + path);
            return this.Decoder.Decode(data);
        }

        public void SetCache(string key, byte[] data)
        {
            this.SetCacheByPath(this.CacheLocator.CreateCachePath(key), data);
        }

        protected void SetCacheByPath(string path, byte[] data)
        {
            byte[] writeData = this.Encoder.Encode(data);
            IO.MakeParentDirectory(this.RootDirectory + path);
            IO.Write(this.RootDirectory + path, writeData);
        }

        public bool HasCache(string key)
        {
            return this.HasCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        protected bool HasCacheByPath(string path)
        {
            return File.Exists(this.RootDirectory + path);
        }

        public IEnumerable<string> ListPathes()
        {
            return new List<string>(
                IO.RecursiveListFiles(this.RootDirectory)
                    .Select(fullpath => fullpath.Replace(this.RootDirectory, ""))
            );
        }

        private void BuildQueue(GameObject obj)
        {
            this.AsyncSetCommandGetCacheByPath(
                    this.RequestQueue.Where(command => this.HasCacheByPath(command.Path))
                )
                .Subscribe(command => this.ResultQueue.OnNext(command))
                .AddTo(obj);

            this.RequestQueue
                .Where(command => !this.HasCacheByPath(command.Path))
                .SelectMany(command =>
                    this.Handler.Fetch(command.Url)
                        .Select(data => command.SetData(data))
                )
                .Do(command => this.AsyncDeleteAndSetCache(obj, command))
	            .Catch<Command, Exception>(exception =>
	            {
		            this.ResultQueue.OnError(exception);
		            return Observable.Never<Command>();
	            })
                .Subscribe(command => this.ResultQueue.OnNext(command))
                .AddTo(obj);
        }

        protected virtual IObservable<Command> AsyncSetCommandGetCacheByPath(
            IObservable<Command> observable)
        {
            return observable
                    .ObserveOn(Scheduler.ThreadPool)
                    .Select(command => this.SetCommandGetCacheByPath(command))
                    .ObserveOnMainThread()
                ;
        }

        protected Command SetCommandGetCacheByPath(Command command)
        {
            try
            {
                command.SetData(this.GetCacheByPath(command.Path));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error AsyncGetCacheByPath: " + e.Message);
            }

            return command;
        }

        protected virtual void AsyncDeleteAndSetCache(GameObject obj, Command command)
        {
            Observable.Return(command)
                .ObserveOn(Scheduler.ThreadPool)
                .Subscribe(_command => this.DeleteAndSetCache(_command))
                .AddTo(obj);
        }

        protected void DeleteAndSetCache(Command command)
        {
            try
            {
                var allPathes = ListPathes();
                var keyPathes = new List<string>(this.CacheLocator.GetSameKeyCachePathes(command.Key, allPathes));
                var excludePath = command.Path;

                foreach (var path in keyPathes)
                {
                    if (excludePath.Equals(path)) continue;
                    this.DeleteByPath(path);
                }

                this.SetCacheByPath(command.Path, command.Data);
            }
            catch (DirectoryNotFoundException)
            {
            }
        }
    }
}
