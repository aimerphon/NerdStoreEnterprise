using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Carrinho.API.Migrations
{
    public partial class Voucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                schema: "nse",
                table: "CarrinhoClientes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "VoucherUtilizado",
                schema: "nse",
                table: "CarrinhoClientes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCodigo",
                schema: "nse",
                table: "CarrinhoClientes",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentual",
                schema: "nse",
                table: "CarrinhoClientes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDesconto",
                schema: "nse",
                table: "CarrinhoClientes",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorDesconto",
                schema: "nse",
                table: "CarrinhoClientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desconto",
                schema: "nse",
                table: "CarrinhoClientes");

            migrationBuilder.DropColumn(
                name: "VoucherUtilizado",
                schema: "nse",
                table: "CarrinhoClientes");

            migrationBuilder.DropColumn(
                name: "VoucherCodigo",
                schema: "nse",
                table: "CarrinhoClientes");

            migrationBuilder.DropColumn(
                name: "Percentual",
                schema: "nse",
                table: "CarrinhoClientes");

            migrationBuilder.DropColumn(
                name: "TipoDesconto",
                schema: "nse",
                table: "CarrinhoClientes");

            migrationBuilder.DropColumn(
                name: "ValorDesconto",
                schema: "nse",
                table: "CarrinhoClientes");
        }
    }
}
