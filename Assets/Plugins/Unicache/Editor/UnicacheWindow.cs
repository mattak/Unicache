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

        void OnGUI()
        {
            GUILayout.Label("Setting", EditorStyles.boldLabel);
            this.Selected = this.LoadObject();
            this.Selected =
                EditorGUILayout.ObjectField(
                    "IUnicacheGetter",
                    this.Selected,
                    typeof(GameObject),
                    true
                ) as GameObject;

            var getter = (this.Selected != null)
                ? this.Selected.GetComponent<IUnicacheGetter>()
                : null;

            if (getter != null)
            {
                var id = this.Selected.gameObject.GetInstanceID();
                EditorPrefs.SetInt(ICacheGetterGameObjectId, id);
                this.Cache = getter.Cache;
            }
            else
            {
                this.Selected = null;
                this.Cache = null;
            }

            if (this.Selected == null)
            {
                EditorGUILayout.HelpBox(
                    "Please Set GameObject which component implement IUnicacheGetter",
                    MessageType.None);
            }

            GUILayout.Label("Cache", EditorStyles.boldLabel);

            if (this.Cache != null)
            {
                this.RenderCachePathHeader();

                foreach (var path in this.Cache.ListPathes())
                {
                    this.RenderCachePath(path);
                }
            }
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