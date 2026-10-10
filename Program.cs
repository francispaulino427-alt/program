using System;
using Spectre.Console;

class Program
{
    static void Main()
    {
        // Titulo del programa
        AnsiConsole.Write(
            new FigletText("Amortizacion")
                .Centered()
                .Color(Color.Green));

        AnsiConsole.MarkupLine(
            "[bold yellow]CALCULO DE PRESTAMO CON CUOTAS FIJAS[/]\n");

        // Solicitar los datos al usuario
        decimal monto = AnsiConsole.Ask<decimal>(
            "Ingrese el monto del prestamo: ");

        decimal tasaAnual = AnsiConsole.Ask<decimal>(
            "Ingrese la tasa de interes anual (%): ");

        int plazo = AnsiConsole.Ask<int>(
            "Ingrese el plazo del prestamo en meses: ");

        // Validar los datos ingresados
        if (monto <= 0 || tasaAnual < 0 || plazo <= 0)
        {
            AnsiConsole.MarkupLine(
                "[red]Error: el monto y el plazo deben ser positivos, y la tasa no puede ser negativa.[/]");

            return;
        }

        // Calcular la tasa de interes mensual
        decimal tasaMensual = tasaAnual / 100m / 12m;

        // Calcular la cuota fija mensual
        decimal cuota;

        if (tasaMensual == 0)
        {
            // Si no hay intereses, se divide el monto entre los meses
            cuota = monto / plazo;
        }
        else
        {
            // Aplicar la formula de amortizacion
            double i = (double)tasaMensual;
            double potencia = Math.Pow(1 + i, plazo);

            cuota = monto * (decimal)(
                i * potencia / (potencia - 1));
        }

        // Redondear la cuota a dos decimales
        cuota = Math.Round(cuota, 2);

        // Mostrar la cuota mensual
        AnsiConsole.MarkupLine(
            $"\n[bold green]Cuota fija mensual: {cuota:N2}[/]\n");

        // Crear la tabla de amortizacion
        var tabla = new Table();

        tabla.Title("[bold yellow]TABLA DE AMORTIZACION[/]");
        tabla.Border(TableBorder.Rounded);

        tabla.AddColumn("No. cuota");
        tabla.AddColumn("Pago de cuota");
        tabla.AddColumn("Interes a pagar");
        tabla.AddColumn("Abono a capital");
        tabla.AddColumn("Saldo pendiente");

        // Inicializar el saldo del prestamo
        decimal saldo = monto;

        // Calcular cada cuota con un ciclo for
        for (int mes = 1; mes <= plazo; mes++)
        {
            // Calcular el interes del periodo
            decimal interes = Math.Round(
                saldo * tasaMensual, 2);

            // Calcular el abono al capital
            decimal abonoCapital = cuota - interes;

            // El pago normal es la cuota fija
            decimal pago = cuota;

            // Ajustar la ultima cuota para liquidar la deuda
            if (mes == plazo || abonoCapital >= saldo)
            {
                abonoCapital = saldo;
                pago = interes + abonoCapital;
            }

            // Actualizar el saldo pendiente
            saldo = Math.Round(
                saldo - abonoCapital, 2);

            // Evitar que quede un saldo residual por redondeo
            if (saldo < 0.01m)
            {
                saldo = 0;
            }

            // Agregar los datos del mes a la tabla
            tabla.AddRow(
                mes.ToString(),
                pago.ToString("N2"),
                interes.ToString("N2"),
                abonoCapital.ToString("N2"),
                saldo.ToString("N2"));
        }

        // Mostrar la tabla completa en la consola
        AnsiConsole.Write(tabla);

        // Mensaje final
        AnsiConsole.MarkupLine(
            "\n[bold green]Amortizacion calculada correctamente.[/]");

        AnsiConsole.MarkupLine(
            "[grey]Gracias por utilizar el programa.[/]");
    }
}