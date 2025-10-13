using Syncfusion.Maui.Core.Hosting;
using Microsoft.Extensions.Logging;
using Mapidemic.Models;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microcharts.Maui;
using Syncfusion.Licensing;

namespace Mapidemic;

public static class MauiProgram
{
	private const string sfKey = "Ngo9BigBOggjGyl/Vkd+XU9FcVRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3tSdkRiWH9dcHFRT2ZbVU91Xg==";
	public static BusinessLogic businessLogic = new BusinessLogic(new Database());
	public static MauiApp CreateMauiApp()
	{
		SyncfusionLicenseProvider.RegisterLicense(sfKey);
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMicrocharts()
			.ConfigureSyncfusionCore()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.UseMauiMaps(); // Enable map functionality

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
