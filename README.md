# MouseGlobeHook

## 概述 | Overview
`MouseGlobeHook` 是一個使用 C# 編寫的 Windows Forms 應用程式，利用 Windows API 掛鉤來捕捉和處理全域滑鼠和鍵盤事件。此應用程式會即時記錄滑鼠移動和複合鍵組合，並在除錯控制台中輸出。

`MouseGlobeHook` is a Windows Forms application written in C# that captures and processes global mouse and keyboard events using Windows API hooks. The application logs mouse movement and complex key combinations in real-time and outputs them to the debug console.

## 功能 | Features
- 捕捉全域滑鼠事件（例如移動和點擊）。
- 捕捉全域鍵盤事件，包括修飾鍵狀態（例如 Ctrl、Shift、Alt）。
- 即時輸出按下的複合鍵和滑鼠位置到除錯控制台。
- 表單關閉時解除掛鉤並釋放資源，確保正確清理。

- Captures global mouse events (e.g., movements and clicks).
- Captures global keyboard events, including modifier key states (e.g., Ctrl, Shift, Alt).
- Displays real-time key combinations pressed and mouse locations in the debug output.
- Unhooks and releases resources when the form is closed to ensure proper cleanup.

## 安裝 | Installation
1. 確保已安裝 .NET Framework（與本專案相容的版本）。
2. 複製或下載此存儲庫。
3. 在 Visual Studio 中打開方案。
4. 建置並執行專案。


1. Ensure that you have .NET Framework installed (compatible with the version targeted by this project).
2. Clone or download the repository.
3. Open the solution in Visual Studio.
4. Build and run the project.

## 使用方法 | Usage
1. 執行應用程式。它將立即開始捕捉鍵盤和滑鼠事件。
2. 按下各種複合鍵以在除錯控制台中查看其記錄。
3. 移動滑鼠以在除錯控制台中查看其座標。


1. Run the application. It will start capturing keyboard and mouse events immediately.
2. Press various key combinations to see them logged in the debug console.
3. Move the mouse to see the coordinates logged in the debug console.

### 鍵盤組合 | Key Combinations
- 此應用程式會檢測到 **Ctrl**、**Shift** 和 **Alt** 與其他鍵一起按下，並以 `Ctrl+Shift+Key` 格式記錄組合。
- 未包含修飾鍵的簡單鍵按下也會被記錄。

- The application detects when **Ctrl**, **Shift**, and **Alt** are pressed along with other keys and logs the combination in the format `Ctrl+Shift+Key`.
- Simple key presses without modifiers are also logged.

### 滑鼠移動 | Mouse Movements
- 當發生滑鼠事件時，應用程式會記錄滑鼠當前在螢幕上的位置。

- The application logs the mouse's current location on the screen whenever a mouse event occurs.

## 技術細節 | Technical Details
- 應用程式使用 **Windows API** 函數，如 `SetWindowsHookEx`、`UnhookWindowsHookEx` 和 `CallNextHookEx` 設置掛鉤和處理回調。
- `GetModuleHandle` 用於取得當前進程模組的句柄。
- 結構 `KBDLLHOOKSTRUCT` 和 `MSLLHOOKSTRUCT` 用於處理低級鍵盤和滑鼠掛鉤事件。
- 當表單關閉時，使用 `UnhookWindowsHookEx` 來移除掛鉤，避免資源洩漏。

- The application uses **Windows API** functions such as `SetWindowsHookEx`, `UnhookWindowsHookEx`, and `CallNextHookEx` to set hooks and handle callbacks.
- `GetModuleHandle` is used to retrieve the handle for the current process module.
- Structs `KBDLLHOOKSTRUCT` and `MSLLHOOKSTRUCT` are used to process low-level keyboard and mouse hook events.
- The hooks are removed using `UnhookWindowsHookEx` when the form is closed to avoid resource leaks.

### Windows API 參考 | Windows API References
- **user32.dll**
  - `SetWindowsHookEx`
  - `UnhookWindowsHookEx`
  - `CallNextHookEx`
- **kernel32.dll**
  - `GetModuleHandle`

## 程式碼結構 | Code Structure
### 重要方法 | Important Methods
- **SetKeyboardHook**：初始化鍵盤掛鉤並返回其指標。
- **KeyboardHookCallback**：處理全域鍵盤事件，檢測修飾鍵並記錄複合鍵。
- **SetHook**：初始化滑鼠掛鉤並返回其指標。
- **HookCallback**：處理全域滑鼠事件並記錄滑鼠位置。

- **SetKeyboardHook**: Initializes a keyboard hook and returns its pointer.
- **KeyboardHookCallback**: Handles global keyboard events, checks for modifier keys, and logs key combinations.
- **SetHook**: Initializes a mouse hook and returns its pointer.
- **HookCallback**: Handles global mouse events and logs the mouse's position.

### 欄位 | Fields
- `hookID` 和 `keyboardHookID`：存儲已安裝掛鉤的指標。
- `isCtrlPressed`、`isShiftPressed`、`isAltPressed`：布林標誌，用於指示修飾鍵的狀態。

- `hookID` and `keyboardHookID`: Store the pointers to the installed hooks.
- `isCtrlPressed`, `isShiftPressed`, `isAltPressed`: Boolean flags indicating the state of modifier keys.

### 事件記錄 | Event Logging
- 使用 `Debug.WriteLine` 記錄事件數據，如複合鍵和滑鼠座標。

- `Debug.WriteLine` is used for logging event data, such as key combinations and mouse coordinates.

## 清理 | Cleanup
- `OnFormClosing` 方法確保應用程式關閉時解除掛鉤並釋放資源。

- The `OnFormClosing` method ensures that hooks are unhooked and resources are properly released when the application is closed.

## 限制 | Limitations
- 此應用程式需要管理員權限才能捕捉全域鍵盤和滑鼠事件。
- 目前僅提供除錯控制台的基本記錄；如有需要，可實現更先進的記錄機制。

- This application requires administrative privileges to capture global keyboard and mouse events.
- Only basic logging to the debug output is provided; a more sophisticated logging mechanism can be implemented as needed.



## 作者 | Author
- q020385791

如有問題、提交問題或建議，歡迎提供貢獻。

Feel free to contribute by submitting issues, pull requests, or suggestions.
```
