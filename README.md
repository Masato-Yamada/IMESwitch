\# IMESwitcher



A lightweight background resident application for Windows that switches IME using a custom hotkey.

Designed to run silently in the system tray without showing any window or taskbar entry.



\## ✨ Features



\- Runs as a \*\*background resident application\*\* (no taskbar icon)

\- Provides a \*\*system tray icon\*\* with a simple menu

\- Switches IME when pressing \*\*Left Ctrl + Space\*\*

\- Uses a \*\*low-level keyboard hook\*\* (WH\_KEYBOARD\_LL)

\- Supports \*\*localized resources\*\* (English / Japanese)

\- Extremely small and fast — ideal for daily use



\## 🔧 Hotkey Behavior



When the following keys are pressed:



\- \*\*Left Ctrl + Space\*\*



The app simulates:



\- Win key down

\- Space key down/up

\- Win key up



This triggers Windows' IME switching behavior.



\## 📂 Project Structure

IMESwitcher/

├── IMESwitcher.csproj

├── Program.cs

├── HiddenForm.cs

├── Properties/

│   ├── Resources.resx

│   ├── Resources.ja.resx

│   ├── Resources.en.resx

│   ├── Resources.Designer.cs

│   └── AssemblyInfo.cs

└── README.md



\## 🚀 How to Build



1\. Open the project in \*\*Visual Studio\*\*

2\. Make sure the project type is set to \*\*Windows Application\*\*

3\. Build the project (`Ctrl + Shift + B`)

4\. The executable will appear under:



\## 🖥️ How It Works



The app:



1\. Hides the console window  

2\. Creates a tray icon  

3\. Installs a low-level keyboard hook  

4\. Runs an invisible form to keep the message loop alive  

5\. Switches IME when the hotkey is pressed  



All processing happens in the background.



\## 📜 License



MIT License  

Feel free to use, modify, and distribute.



\## 🙌 Author



Masato (雅人)

