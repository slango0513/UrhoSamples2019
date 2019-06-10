using Urho;
using Urho.Gui;

namespace BugWorkarounds
{
    public class ScreenKeyboardBug : Application
    {
        public ScreenKeyboardBug(ApplicationOptions options) : base(options)
        {
        }

        protected override void Start()
        {
            var ui = UI;

            ui.Scale = 2f;
            ui.Root.SetDefaultStyle(CoreAssets.UIs.DefaultStyle);

            // Create the Window and add it to the UI's root node
            var window = new Window();
            ui.Root.AddChild(window);

            // Set Window size and layout settings
            window.SetMinSize(384, 192);
            window.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
            window.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            window.Name = "Window";

            // Create Window 'titlebar' container
            var titleBar = new UIElement();
            titleBar.SetMinSize(0, 24);
            titleBar.VerticalAlignment = VerticalAlignment.Top;
            titleBar.LayoutMode = LayoutMode.Horizontal;

            // Create the Window title Text
            var windowTitle = new Text
            {
                Name = "WindowTitle",
                Value = "Hello GUI!"
            };

            // Create the Window's close button
            var buttonClose = new Button
            {
                Name = "CloseButton"
            };

            // Add the controls to the title bar
            titleBar.AddChild(windowTitle);
            titleBar.AddChild(buttonClose);

            // Add the title bar to the Window
            window.AddChild(titleBar);

            // Apply styles
            window.SetStyleAuto(null);
            windowTitle.SetStyleAuto(null);
            buttonClose.SetStyle("CloseButton", null);

            buttonClose.Released += args =>
            {
                window.Visible = false;
            };

            // Subscribe also to all UI mouse clicks just to see where we have clicked
            UI.UIMouseClick += args =>
            {
            };


            for (int i = 0; i < 4; i++)
            {
                // Create a CheckBox
                var checkBox = new CheckBox
                {
                    Name = "CheckBox"
                };
                // Add controls to Window
                window.AddChild(checkBox);
                // Apply previously set default style
                checkBox.SetStyleAuto(null);
            }

            for (int i = 0; i < 4; i++)
            {
                // Create a Button
                var button = new Button
                {
                    Name = "Button",
                    MinHeight = 24
                };
                window.AddChild(button);
                button.SetStyleAuto(null);
            }

            for (int i = 0; i < 8; i++)
            {
                // Create a LineEdit
                var lineEdit = new LineEdit
                {
                    Name = "LineEdit",
                    MinHeight = 24
                };
                window.AddChild(lineEdit);
                lineEdit.SetStyleAuto(null);
                // TODO
                //lineEdit.Focused += args =>
                //{
                //};
            }
        }
    }
}
