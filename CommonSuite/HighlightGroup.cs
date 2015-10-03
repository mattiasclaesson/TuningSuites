using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using NLog;

namespace CommonSuite
{
    public class HighlightGroup : IDisposable
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        List<TextMarker> _markers = new List<TextMarker>();
        TextEditorControl _editor;
        IDocument _document;
        public HighlightGroup(TextEditorControl editor)
        {
            _editor = editor;
            _document = editor.Document;
        }
        public void AddMarker(TextMarker marker)
        {
            _markers.Add(marker);
            _document.MarkerStrategy.AddMarker(marker);
        }
        public void ClearMarkers()
        {
            foreach (TextMarker m in _markers)
                _document.MarkerStrategy.RemoveMarker(m);
            _markers.Clear();
            _editor.Refresh();
        }

        public void Dispose()
        {
            logger.Debug("Disposed!");
            ClearMarkers();
            GC.SuppressFinalize(this);
        }

        ~HighlightGroup() { Dispose(); }

        public IList<TextMarker> Markers { get { return _markers.AsReadOnly(); } }
    }
}
