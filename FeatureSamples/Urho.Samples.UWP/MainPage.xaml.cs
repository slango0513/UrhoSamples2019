using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Urho.Samples.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Application currentApplication;
        TypeInfo selectedGameType;

        public MainPage()
        {
            Urho.Application.UnhandledException += (s, e) => e.Handled = true;
            InitializeComponent();
            GameTypes = typeof(Sample).GetTypeInfo().Assembly.GetTypes()
                .Where(t => t.GetTypeInfo().IsSubclassOf(typeof(Application))
                    && t != typeof(Sample) && t != typeof(BasicTechniques)
                    && t != typeof(PBRMaterials) && t != typeof(DynamicGeometry))
                .Select((t, i) => new TypeInfo(t, $"{i + 1}. {t.Name}", ""))
                .ToArray();
            DataContext = this;
        }

        public TypeInfo[] GameTypes { get; set; }

        public TypeInfo SelectedGameType
        {
            get { return selectedGameType; }
            set { RunGame(value); selectedGameType = value; }
        }

        public async void RunGame(TypeInfo value)
        {
            currentApplication?.Exit();
            //at this moment, UWP supports assets only in pak files (see PackageTool)
            currentApplication = UrhoSurface.Run(value.Type, new ApplicationOptions("Data") { Width = (int)UrhoSurface.ActualWidth, Height = (int)UrhoSurface.ActualHeight });
        }
    }

    public class TypeInfo
    {
        public Type Type { get; }
        public string Name { get; }
        public string Description { get; }

        public TypeInfo(Type type, string name, string description)
        {
            Type = type;
            Name = name;
            Description = description;
        }
    }
}
