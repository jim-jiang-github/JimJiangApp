using JimJiangApp.Commons;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Popups;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JimJiangApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            if (AppWindowTitleBar.IsCustomizationSupported() is true)
            {
                IntPtr hWnd = WindowNative.GetWindowHandle(this);
                WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
                appWindow.SetIcon(@"Assets\jimjiangapp.ico");
            }
            root.Loaded += Root_Loaded;
        }

        private async void Root_Loaded(object sender, RoutedEventArgs e)
        {
            await DownloadAndInstallAllUpdatesAsync();
        }

        private StoreContext context = null;

        public async Task DownloadAndInstallAllUpdatesAsync()
        {
            Log.GetLogger(this).Information("start");
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }
            Log.GetLogger(this).Information("checking");

            // Get the updates that are available.
            IReadOnlyList<StorePackageUpdate> updates =
                await context.GetAppAndOptionalStorePackageUpdatesAsync();
            Log.GetLogger(this).Information("get list");

            if (updates.Count > 0)
            {
                Log.GetLogger(this).Information("Has update");
                // Alert the user that updates are available and ask for their consent
                // to start the updates.
                MessageDialog dialog = new MessageDialog(
                    "Download and install updates now? This may cause the application to exit.", "Download and Install?");
                dialog.Commands.Add(new UICommand("Yes"));
                dialog.Commands.Add(new UICommand("No"));
                IUICommand command = await dialog.ShowAsync();

                if (command.Label.Equals("Yes", StringComparison.CurrentCultureIgnoreCase))
                {
                    // Download and install the updates.
                    IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                        context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

                    // The Progress async method is called one time for each step in the download
                    // and installation process for each package in this request.
                    downloadOperation.Progress = async (asyncInfo, progress) =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () =>
                        {
                            downloadProgressBar.Value = progress.PackageDownloadProgress;
                        });
                    };

                    StorePackageUpdateResult result = await downloadOperation.AsTask();
                }
            }
            else
            {
                Log.GetLogger(this).Information("No update");
                MessageDialog dialog = new MessageDialog("No update");
                await dialog.ShowAsync();
            }
        }
    }
}
