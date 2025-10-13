using Microsoft.Extensions.Logging;
using Mapidemic.Models;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microcharts.Maui;

namespace Mapidemic;

public static class MauiProgram
{
	public static BusinessLogic businessLogic = new BusinessLogic(new Database());
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMicrocharts()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
