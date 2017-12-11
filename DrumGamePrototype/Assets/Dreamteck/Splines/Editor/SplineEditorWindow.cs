using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Dreamteck.Splines {
    public class SplineEditorWindow : EditorWindow
    {
        protected Editor editor;
        protected SplineEditor splineEditor;

        public void Init(Editor e, string inputTitle, Vector2 min, Vector2 max)
        {
            Init(e, inputTitle);
            minSize = min;
            maxSize = max;
        }

        public void Init(Editor e, Vector2 min, Vector2 max)
        {
            Init(e);
            minSize = min;
            maxSize = max;
        }

        public void Init(Editor e, Vector2 size)
        {
            Init(e);
            minSize = maxSize = size;
        }

        public void Init(Editor e, string inputTitle)
        {
            Init(e);
            Title(inputTitle);
        }

        public void Init(Editor e)
        {
            editor = e;
            if (editor is SplineEditor) splineEditor = (SplineEditor)editor;
            else splineEditor = null;
            Title(GetTitle());
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {

        }

        protected virtual string GetTitle()
        {
            return "Spline Editor Window";
        }

        private void Title(string inputTitle)
        {
#if UNITY_5_0
            title = inputTitle;
#else
            titleContent = new GUIContent(inputTitle);
#endif
        }
    }
}
