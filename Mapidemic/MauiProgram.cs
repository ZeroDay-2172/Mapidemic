using Microsoft.Extensions.Logging;
using Mapidemic.Models;

namespace Mapidemic;

public static class MauiProgram
{
	public static BusinessLogic businessLogic = new BusinessLogic(new Database());
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
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
