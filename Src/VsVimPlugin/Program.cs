using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Vim;
using Vim.Interpreter;
using Vim.VisualStudio.Implementation.CSharpPlugin;

namespace VsVimPlugin;

class Program
{
    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        var fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\."));
        var runner = new CSharpPluginExecutor();
        var a = new Vim();
        runner.LoadAllPlugins(a);
        runner.Execute(new VimBuffer(), new CallInfo("VsVimPlugin.Test",string.Empty, LineRangeSpecifier.None, true), false);
        Console.WriteLine();
        Console.ReadKey();
    }

    private class Vim : IVim
    {
        public IVimBuffer CreateVimBuffer(ITextView textView)
        {
            throw new NotImplementedException();
        }

        public IVimBuffer CreateVimBufferWithData(IVimBufferData vimBufferData)
        {
            throw new NotImplementedException();
        }

        public IVimTextBuffer CreateVimTextBuffer(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        public void CloseAllVimBuffers()
        {
            throw new NotImplementedException();
        }

        public IVimInterpreter GetVimInterpreter(IVimBuffer vimBuffer)
        {
            throw new NotImplementedException();
        }

        public IVimBuffer GetOrCreateVimBuffer(ITextView textView)
        {
            throw new NotImplementedException();
        }

        public IVimTextBuffer GetOrCreateVimTextBuffer(ITextBuffer textBuffer)
        {
            throw new NotImplementedException();
        }

        public VimRcState LoadVimRc()
        {
            throw new NotImplementedException();
        }

        public void LoadSessionData()
        {
            throw new NotImplementedException();
        }

        public void SaveSessionData()
        {
            throw new NotImplementedException();
        }

        public bool RemoveVimBuffer(ITextView obj0)
        {
            throw new NotImplementedException();
        }

        public bool TryGetVimBuffer(ITextView textView, out IVimBuffer vimBuffer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetVimTextBuffer(ITextBuffer textBuffer, out IVimTextBuffer vimTextBuffer)
        {
            throw new NotImplementedException();
        }

        public bool ShouldCreateVimBuffer(ITextView textView)
        {
            throw new NotImplementedException();
        }

        public bool TryGetOrCreateVimBufferForHost(ITextView textView, out IVimBuffer vimBuffer)
        {
            throw new NotImplementedException();
        }

        public FSharpOption<IVimBuffer> TryGetRecentBuffer(int n)
        {
            throw new NotImplementedException();
        }

        public FSharpOption<IVimBuffer> ActiveBuffer { get; }
        public IStatusUtil ActiveStatusUtil { get; }
        public bool AutoLoadDigraphs { get; set; }
        public bool AutoLoadVimRc { get; set; }
        public bool AutoLoadSessionData { get; set; }
        public FSharpList<IVimBuffer> VimBuffers { get; }
        public FSharpOption<IVimBuffer> FocusedBuffer { get; }
        public bool IsDisabled { get; set; }
        public bool InBulkOperation { get; }
        public IDigraphMap DigraphMap { get; }
        public IMacroRecorder MacroRecorder { get; }
        public IMarkMap MarkMap { get; }
        public IRegisterMap RegisterMap { get; }
        public ISearchService SearchService { get; }
        public IVimGlobalAbbreviationMap GlobalAbbreviationMap { get; }
        public IVimGlobalKeyMap GlobalKeyMap { get; }
        public IVimGlobalSettings GlobalSettings { get; }
        public Dictionary<string, VariableValue> VariableMap { get; }
        public IVimData VimData { get; }
        public IVimHost VimHost { get; }
        public VimRcState VimRcState { get; }
    }
    private class VimBuffer : IVimBuffer
    {
        public PropertyCollection Properties { get; }
        public Register GetRegister(RegisterName obj0)
        {
            throw new NotImplementedException();
        }

        public IMode GetMode(ModeKind obj0)
        {
            throw new NotImplementedException();
        }

        public KeyMappingResult GetKeyInputMapping(KeyInput keyInput)
        {
            throw new NotImplementedException();
        }

        public ProcessResult Process(KeyInput obj0)
        {
            throw new NotImplementedException();
        }

        public void ProcessBufferedKeyInputs()
        {
            throw new NotImplementedException();
        }

        public bool CanProcess(KeyInput obj0)
        {
            throw new NotImplementedException();
        }

        public bool CanProcessAsCommand(KeyInput obj0)
        {
            throw new NotImplementedException();
        }

        public IMode SwitchMode(ModeKind obj0, ModeArgument obj1)
        {
            throw new NotImplementedException();
        }

        public IMode SwitchPreviousMode()
        {
            throw new NotImplementedException();
        }

        public void SimulateProcessed(KeyInput obj0)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMode> AllModes { get; }
        public FSharpList<KeyInput> BufferedKeyInputs { get; }
        public FSharpOption<string> CurrentDirectory { get; set; }
        public ICommandUtil CommandUtil { get; }
        public IVimGlobalSettings GlobalSettings { get; }
        public IIncrementalSearch IncrementalSearch { get; }
        public bool IsProcessingInput { get; }
        public bool IsSwitchingMode { get; }
        public bool IsClosed { get; }
        public IJumpList JumpList { get; }
        public FSharpOption<string> LastMessage { get; }
        public IVimLocalSettings LocalSettings { get; }
        public IVimLocalAbbreviationMap LocalAbbreviationMap { get; }
        public IVimLocalKeyMap LocalKeyMap { get; }
        public IMarkMap MarkMap { get; }
        public IMode Mode { get; }
        public ModeKind ModeKind { get; }
        public string Name { get; }
        public FSharpOption<ModeKind> InOneTimeCommand { get; }
        public IRegisterMap RegisterMap { get; }
        public ITextBuffer TextBuffer { get; }
        public ITextSnapshot TextSnapshot { get; }
        public ITextView TextView { get; }
        public IMotionUtil MotionUtil { get; }
        public IUndoRedoOperations UndoRedoOperations { get; }
        public IVim Vim { get; }
        public IVimTextBuffer VimTextBuffer { get; }
        public IVimBufferData VimBufferData { get; }
        public ITextStructureNavigator WordNavigator { get; }
        public IVimWindowSettings WindowSettings { get; }
        public IVimData VimData { get; }
        public INormalMode NormalMode { get; }
        public ICommandMode CommandMode { get; }
        public IDisabledMode DisabledMode { get; }
        public IVisualMode VisualCharacterMode { get; }
        public IVisualMode VisualLineMode { get; }
        public IVisualMode VisualBlockMode { get; }
        public IInsertMode InsertMode { get; }
        public IInsertMode ReplaceMode { get; }
        public ISelectMode SelectCharacterMode { get; }
        public ISelectMode SelectLineMode { get; }
        public ISelectMode SelectBlockMode { get; }
        public ISubstituteConfirmMode SubstituteConfirmMode { get; }
        public IMode ExternalEditMode { get; }
        public bool IsReadOnly { get; }
        public event EventHandler<SwitchModeEventArgs> SwitchedMode;
        public event EventHandler<KeyInputStartEventArgs> KeyInputStart;
        public event EventHandler<KeyInputStartEventArgs> KeyInputProcessing;
        public event EventHandler<KeyInputProcessedEventArgs> KeyInputProcessed;
        public event EventHandler<KeyInputSetEventArgs> KeyInputBuffered;
        public event EventHandler<KeyInputEventArgs> KeyInputEnd;
        public event EventHandler<StringEventArgs> WarningMessage;
        public event EventHandler<StringEventArgs> ErrorMessage;
        public event EventHandler<StringEventArgs> StatusMessage;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler PostClosed;
    }
}