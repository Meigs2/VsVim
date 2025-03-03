﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using AppKit;

namespace Vim.UI.Cocoa.Implementation.Misc
{
    [Export(typeof(IKeyUtil))]
    internal sealed class AlternateKeyUtil : IKeyUtil
    {
        private static readonly Dictionary<NSKey, KeyInput> s_CocoaKeyToKeyInputMap;
        private static readonly Dictionary<NSKey, KeyInput> s_CocoaControlKeyToKeyInputMap;
        private static readonly Dictionary<VimKey, NSKey> s_vimKeyToCocoaKeyMap;
        private static readonly Dictionary<KeyInput, NSKey> s_keyInputToCocoaKeyMap;

        private readonly byte[] _keyboardState = new byte[256];

        static AlternateKeyUtil()
        {
            s_vimKeyToCocoaKeyMap = BuildVimKeyToCocoaKeyMap();
            s_keyInputToCocoaKeyMap = BuildKeyInputToCocoaKeyMap(s_vimKeyToCocoaKeyMap);
            s_CocoaKeyToKeyInputMap = new Dictionary<NSKey, KeyInput>();
            foreach (var pair in s_keyInputToCocoaKeyMap)
            {
                s_CocoaKeyToKeyInputMap[pair.Value] = pair.Key;
            }
            s_CocoaControlKeyToKeyInputMap = BuildCocoaControlKeyToKeyInputMap();
        }

        private static Dictionary<NSKey, KeyInput> BuildCocoaControlKeyToKeyInputMap()
        {
            var map = new Dictionary<NSKey, KeyInput>
            {
                [NSKey.D2] = KeyInputUtil.VimKeyToKeyInput(VimKey.Null), // <C-@>
                [NSKey.D6] = KeyInputUtil.CharToKeyInput((char)0x1E), // <C-^>
                [NSKey.Minus] = KeyInputUtil.CharToKeyInput((char)0x1F), // <C-_>
                //[NSKey.Question] = KeyInputUtil.CharToKeyInput((char)0x7F), // <C-?>
            };

            return map;
        }

        internal static Dictionary<VimKey, NSKey> BuildVimKeyToCocoaKeyMap()
        {
            var map = new Dictionary<VimKey, NSKey>
            {
                [VimKey.Enter] = NSKey.Return,
                [VimKey.Escape] = NSKey.Escape,
                [VimKey.Back] = NSKey.Delete,
                //[VimKey.Delete] = NSKey.Delete,
                [VimKey.Left] = NSKey.LeftArrow,
                [VimKey.Up] = NSKey.UpArrow,
                [VimKey.Right] = NSKey.RightArrow,
                [VimKey.Down] = NSKey.DownArrow,
                [VimKey.Help] = NSKey.Help,
                //[VimKey.Insert] = NSKey.Insert,
                [VimKey.Home] = NSKey.Home,
                [VimKey.End] = NSKey.End,
                [VimKey.PageUp] = NSKey.PageUp,
                [VimKey.PageDown] = NSKey.PageDown,
                [VimKey.Tab] = NSKey.Tab,
                [VimKey.F1] = NSKey.F1,
                [VimKey.F2] = NSKey.F2,
                [VimKey.F3] = NSKey.F3,
                [VimKey.F4] = NSKey.F4,
                [VimKey.F5] = NSKey.F5,
                [VimKey.F6] = NSKey.F6,
                [VimKey.F7] = NSKey.F7,
                [VimKey.F8] = NSKey.F8,
                [VimKey.F9] = NSKey.F9,
                [VimKey.F10] = NSKey.F10,
                [VimKey.F11] = NSKey.F11,
                [VimKey.F12] = NSKey.F12,
                [VimKey.KeypadMultiply] = NSKey.KeypadMultiply,
                [VimKey.KeypadPlus] = NSKey.KeypadPlus,
                [VimKey.KeypadMinus] = NSKey.KeypadMinus,
                [VimKey.KeypadDecimal] = NSKey.KeypadDecimal,
                [VimKey.KeypadDivide] = NSKey.KeypadDivide,
                [VimKey.KeypadEnter] = NSKey.KeypadEnter,
                [VimKey.Keypad0] = NSKey.Keypad0,
                [VimKey.Keypad1] = NSKey.Keypad1,
                [VimKey.Keypad2] = NSKey.Keypad2,
                [VimKey.Keypad3] = NSKey.Keypad3,
                [VimKey.Keypad4] = NSKey.Keypad4,
                [VimKey.Keypad5] = NSKey.Keypad5,
                [VimKey.Keypad6] = NSKey.Keypad6,
                [VimKey.Keypad7] = NSKey.Keypad7,
                [VimKey.Keypad8] = NSKey.Keypad8,
                [VimKey.Keypad9] = NSKey.Keypad9
            };

            return map;
        }

        internal static Dictionary<KeyInput, NSKey> BuildKeyInputToCocoaKeyMap(Dictionary<VimKey, NSKey> vimKeyToCocoaKeyMap)
        {
            var map = new Dictionary<KeyInput, NSKey>();
            foreach (var pair in vimKeyToCocoaKeyMap)
            {
                var keyInput = KeyInputUtil.VimKeyToKeyInput(pair.Key);
                map[keyInput] = pair.Value;
            }

            map[KeyInputUtil.CharToKeyInput(' ')] = NSKey.Space;
            map[KeyInputUtil.CharToKeyInput('\t')] = NSKey.Tab;

            return map;
        }

        internal static bool TrySpecialVimKeyToKey(VimKey vimKey, out NSKey key)
        {
            return s_vimKeyToCocoaKeyMap.TryGetValue(vimKey, out key);
        }

        internal static VimKeyModifiers ConvertToKeyModifiers(NSEventModifierMask keys)
        {
            var res = VimKeyModifiers.None;
            if (keys.HasFlag(NSEventModifierMask.ShiftKeyMask))
            {
                res |= VimKeyModifiers.Shift;
            }
            if (keys.HasFlag(NSEventModifierMask.AlternateKeyMask))
            {
                res |= VimKeyModifiers.Alt;
            }
            if (keys.HasFlag(NSEventModifierMask.ControlKeyMask))
            {
                res |= VimKeyModifiers.Control;
            }
            if (keys.HasFlag(NSEventModifierMask.CommandKeyMask))
            {
                res |= VimKeyModifiers.Command;
            }
            return res;
        }

        internal static NSEventModifierMask ConvertToModifierKeys(VimKeyModifiers keys)
        {
            NSEventModifierMask res = 0;
            if (keys.HasFlag(VimKeyModifiers.Shift))
            {
                res |= NSEventModifierMask.ShiftKeyMask;
            }
            if (keys.HasFlag(VimKeyModifiers.Alt))
            {
                res |= NSEventModifierMask.AlternateKeyMask;
            }
            if (keys.HasFlag(VimKeyModifiers.Control))
            {
                res |= NSEventModifierMask.ControlKeyMask;
            }
            if (keys.HasFlag(VimKeyModifiers.Command))
            {
                res |= NSEventModifierMask.CommandKeyMask;
            }
            return res;
        }

        internal static bool IsAltGr(NSEventModifierMask modifierKeys)
        {
            var altGr = NSEventModifierMask.ControlKeyMask | NSEventModifierMask.AlternateKeyMask;
            return (modifierKeys & altGr) == altGr;
        }

        #region IKeyUtil

        bool IKeyUtil.IsAltGr(NSEventModifierMask modifierKeys)
        {
            return IsAltGr(modifierKeys);
        }

        VimKeyModifiers IKeyUtil.GetKeyModifiers(NSEventModifierMask modifierKeys)
        {
            return ConvertToKeyModifiers(modifierKeys);
        }

        bool IKeyUtil.TryConvertSpecialToKeyInput(NSEvent theEvent, out KeyInput keyInput)
        {
            var key = (NSKey)theEvent.KeyCode;
            NSEventModifierMask modifierKeys = theEvent.ModifierFlags;
            if (s_CocoaKeyToKeyInputMap.TryGetValue(key, out keyInput))
            {
                var keyModifiers = ConvertToKeyModifiers(modifierKeys);
                keyInput = KeyInputUtil.ApplyKeyModifiers(keyInput, keyModifiers);
                return true;
            }

            // Vim allows certain "lazy" control keys, such as <C-6> for <C-^>.
            if ((modifierKeys == NSEventModifierMask.ControlKeyMask ||
                modifierKeys == (NSEventModifierMask.ControlKeyMask | NSEventModifierMask.ShiftKeyMask)) &&
                s_CocoaControlKeyToKeyInputMap.TryGetValue(key, out keyInput))
            {
                return true;
            }

            // If the key doesn't correspond to an ASCII control key (like <C-^>), we need to convert it here.
            // This is needed because key combinations like <C-;> won't be passed to
            // TextInput, because they can't be represented as system or control text.
            // We just have to be careful not to shadow any keys that produce text when
            // combined with the AltGr key.
  
            switch (key)
            {
                case NSKey.Option:
                case NSKey.RightOption:
                case NSKey.Control:
                case NSKey.RightControl:
                case NSKey.Shift:
                case NSKey.RightShift:
                case NSKey.Command:
                    // Avoid work for common cases.
                    break;

                default:
                    VimTrace.TraceInfo("AlternateKeyUtil::TryConvertSpecialKeyToKeyInput {0} {1}",
                        key, modifierKeys);
                    if (GetKeyInputFromKey(theEvent, modifierKeys, out keyInput))
                    {
                        // Only produce a key input here if the key input we
                        // found is *not* an ASCII control character.
                        // Control characters will be handled by TextInput
                        // as control text.


                        // Commented out because we are not using TextInput
                        //if (!System.Char.IsControl(keyInput.Char))
                        {
                            return true;
                        }
                    }
                    break;
            }

            keyInput = null;
            return false;
        }

        private bool GetKeyInputFromKey(NSEvent theEvent, NSEventModifierMask modifierKeys, out KeyInput keyInput)
        {
            if(!string.IsNullOrEmpty(theEvent.CharactersIgnoringModifiers))
            {
                var keyModifiers = ConvertToKeyModifiers(modifierKeys);
                keyInput = KeyInputUtil.ApplyKeyModifiersToChar(theEvent.CharactersIgnoringModifiers[0], keyModifiers);
                return true;
            }
            keyInput = null;
            return false;
        }

        #endregion
    }
}
