#r "Microsoft.VisualStudio.CoreUtility.dll"
#r "Microsoft.VisualStudio.Text.UI.dll"
#r "Microsoft.VisualStudio.Text.UI.Wpf.dll"
#r "VsVim.dll"
#r "Vim.Core.dll"

using system;

//Comment out before use!
public string Name;
public string Arguments;
public Vim.Interpreter.LineRangeSpecifier LineRange;
public bool IsScriptLocal;
public IVim Vim;
public IVimBuffer VimBuffer;

IWpfTextView textView = VimBuffer.TextView as IWpfTextView;
if (textView == null)
{
    VimBuffer.VimBufferData.StatusUtil.OnError("Can not get WpfTextView");
    return;
}

private List<SnapshotSpan> InternalMatchAll(SnapshotSpan snapshot, Regex regex)
{
    var text = snapshot.GetText();
    var capture = regex.Matches(text);

    var result = new List<SnapshotSpan>();

    foreach (Match item in capture)
    {
        if (item.Success)
        {
            var point = SnapshotPointUtil.Add(item.Index, snapshot.Start);
            var span = SnapshotSpanUtil.CreateWithLength(point, item.Length);
            result.Add(span);
        }
    }

    return result;
}

//MessageBox.Show(VimBuffer.TextView.GetVisibleSnapshotSpans().Count.ToString());
//MessageBox.Show(String.Concat(VimBuffer.TextView.GetVisibleSnapshotSpans().Select(s => s.GetText())));
//var visibleMatches = InternalMatchAll(span, new Regex("test"));

//textView.ProvisionalTextHighlight
var b = InternalMatchAll(VimBuffer.TextView.GetVisibleSnapshotSpans()[0], new System.Text.RegularExpressions.Regex("DialogResult"));
VimBuffer.TextView.Caret.MoveTo(b[0].End);
MessageBox.Show(String.Join("\n", b.Select(s => $"{s.Start.Position}, {s.End.Position}")));
//VimBuffer.KeyInputStart += OnKeyInputStart;
//VimBuffer.Closed += OnBufferClosed;

private void OnKeyInputStart(object sender, KeyInputStartEventArgs e)
{
    e.Handled = true;

    if (e.KeyInput.Char == 'j')
    {
        textView.ViewScroller.ScrollViewportVerticallyByPixels(-50);
    }
    else if (e.KeyInput.Char == 'k')
    {
        textView.ViewScroller.ScrollViewportVerticallyByPixels(50);
    }
    else
    {
        var count = textView.TextViewLines.Count;
        var index = count / 2;
        if (textView.TextViewLines.Count <= index)
        {
            index = 0;
        }
        var line = textView.TextViewLines[index];

        var lineNumber = line.Start.GetContainingLine().LineNumber;
        var snapshotLine = textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
        var point = new SnapshotPoint(textView.TextSnapshot, snapshotLine.Start.Position);
        textView.Caret.MoveTo(new SnapshotPoint(textView.TextSnapshot, point));

        EndIntercept();
        return;
    }
}
private void EndIntercept()
{
    VimBuffer.KeyInputStart -= OnKeyInputStart;
    VimBuffer.Closed -= OnBufferClosed;
}
private void OnBufferClosed(object sender, EventArgs e)
{
    EndIntercept();
}