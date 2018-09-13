using UnityEditor;
using UnityEngine;

namespace Unicache
{
    public class UnicacheWindow : EditorWindow
    {
        [MenuItem("Window/Unicache")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<UnicacheWindow>();
            window.Show();
        }

        private GameObject Selected;
        private IUnicache Cache;
        private int SelectedInstanceId = -1;
        private const string ICacheGetterGameObjectId = "UnicacheWindow.IUnicacheGetter";
        private Vector2 _scrollPosition = Vector2.zero;

        void OnGUI()
        {
            GUILayout.Label("Setting", EditorStyles.boldLabel);
            this.Cache = this.RenderSettingSection();

            GUILayout.Label("Cache", EditorStyles.boldLabel);
            this.RenderCacheSection(this.Cache);
        }

        GameObject LoadObject()
        {
            if (this.SelectedInstanceId == -1)
            {
                this.SelectedInstanceId = EditorPrefs.GetInt(ICacheGetterGameObjectId, -1);

                if (this.SelectedInstanceId != -1)
                {
                    return EditorUtility.InstanceIDToObject(this.SelectedInstanceId) as GameObject;
                }
            }

            return this.Selected;
        }

        void SaveObject(GameObject obj)
        {
            var id = obj.GetInstanceID();
            EditorPrefs.SetInt(ICacheGetterGameObjectId, id);
        }

        IUnicache RenderSettingSection()
        {
            this.Selected = this.LoadObject();
            this.Selected =
                EditorGUILayout.ObjectField(
                    "IUnicacheGetter",
                    this.Selected,
                    typeof(GameObject),
                    true
                ) as GameObject;

            IUnicache cache = null;
            var getter = (this.Selected != null)
                ? this.Selected.GetComponent<IUnicacheGetter>()
                : null;

            if (getter != null)
            {
                this.SaveObject(this.Selected);
                cache = getter.Cache;
            }
            else
            {
                this.Selected = null;
                cache = null;
            }

            if (this.Selected == null)
            {
                EditorGUILayout.HelpBox(
                    "Please Set GameObject which component implement IUnicacheGetter",
                    MessageType.None);
            }

            return cache;
        }

        void RenderCacheSection(IUnicache cache)
        {
            if (cache == null)
            {
                return;
            }

            {
                this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition, GUI.skin.box);

                this.RenderCachePathHeader();

                foreach (var path in cache.ListPathes())
                {
                    this.RenderCachePath(path);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        void RenderCachePathHeader()
        {
            EditorGUILayout.BeginHorizontal(GUIStyle.none);

            if (GUILayout.Button("CLEAR", GUILayout.MaxWidth(60), GUILayout.MaxHeight(15)))
            {
                this.Cache.Clear();
            }

            GUILayout.Label("Cache Path", EditorStyles.label);
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        void RenderCachePath(string path)
        {
            EditorGUILayout.BeginHorizontal(GUIStyle.none);

            if (GUILayout.Button("DEL", GUILayout.MaxWidth(60), GUILayout.MaxHeight(15)))
            {
                this.Cache.DeleteByPath(path);
            }

            GUILayout.Label(path, EditorStyles.label);
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }
    }
}