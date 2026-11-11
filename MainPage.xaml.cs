//using Microsoft.Maui.ApplicationModel;
namespace PruebaMaui;
using System.Threading;
public partial class MainPage : ContentPage
{
	private CancellationTokenSource? source;
	public MainPage()
	{
		InitializeComponent();
		btnCancelar.IsEnabled = false;
	}

		private async void OnIniciarClicked(object sender, EventArgs e)
	{
		//Tokens para cancelar la tarea
		source = new CancellationTokenSource();
		CancellationToken token = source.Token;

		//Botones
		btnIniciar.IsEnabled = false;
		btnCancelar.IsEnabled = true;
		//Estado del proceso
		lblEstado.Text = "Iniciando...";
		//barra de progreso
		barraProgreso.Progress = 0;

		try
		{
			await Task.Run(() =>
			{
				for(int i=0; i < 10; i++)
				{
					//Comprobamos si se quiere cancelar la tarea
					token.ThrowIfCancellationRequested();

					//Simula una tarea que tarda 1 segundo
					//Al agregar token a Task.Delay, podemos cancelar la tarea mientras está en espera
					Task.Delay(1000, token).GetAwaiter().GetResult();
					//otra comprobación de cancelación por pura paranoia akakaka
					token.ThrowIfCancellationRequested();

					int registro = i;
					MainThread.BeginInvokeOnMainThread(() =>
					{
						//Cambiamos el estado de la barra de progreso y el label de estado
						lblEstado.Text = $"Progreso: {registro} de 10";
						barraProgreso.Progress = registro / 10.0;
					});
				}
			},token);
			//Proceso completado
			lblEstado.Text = "Proceso completado";
		}
		catch (OperationCanceledException)
		{
			lblEstado.Text = "Proceso cancelado";
		}
		finally
		{
			//Colocamos los botones en su estado inicial
			btnIniciar.IsEnabled = true;
			btnCancelar.IsEnabled = false;

			source.Dispose();
			source = null;
		}

		//Procesamiento antiguo
		// await Task.Run(() =>
		// {
		// 	for (int i = 0; i < 10; i++)
		// 	{
		// 		Thread.Sleep(1000); // Simula una tarea que tarda 1 segundo

		// 		//Actualizar lblEstado desde el hilo principal
		// 		MainThread.BeginInvokeOnMainThread(() =>
		// 		{
		// 			lblEstado.Text = $"Progreso: {i + 1} de 10";
		// 		});

		// 		//Actualizar barra de progreso desde el hilo principal
		// 		MainThread.BeginInvokeOnMainThread(() =>
		// 		{
		// 			barraProgreso.Progress = (double)(i + 1)/10;
		// 		});
		// 	}
		// });

		// //Mostrar procesamiento completado
		// lblEstado.Text = "Procesamiento completado";
		// //Habilitar el botón nuevamente
		// btnIniciar.IsEnabled = true;
	}

	private void OnCancelarClicked(object sender, EventArgs e)
	{
		btnCancelar.IsEnabled = false;
		lblEstado.Text = "Cancelando proceso...";

		//Cancelamos la tarea
		source?.Cancel();
		
	}

}
