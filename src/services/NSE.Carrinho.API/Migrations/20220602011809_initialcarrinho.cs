using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Carrinho.API.Migrations
{
    public partial class initialcarrinho : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nse");

            migrationBuilder.CreateTable(
                name: "CarrinhoClientes",
                schema: "nse",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClienteId = table.Column<Guid>(nullable: false),
                    ValorTotal = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoClientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarrinhoItems",
                schema: "nse",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProdutoId = table.Column<Guid>(nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", nullable: true),
                    Quantidade = table.Column<int>(nullable: false),
                    Valor = table.Column<decimal>(nullable: false),
                    Imagem = table.Column<string>(type: "varchar(100)", nullable: true),
                    CarrinhoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarrinhoItems_CarrinhoClientes_CarrinhoId",
                        column: x => x.CarrinhoId,
                        principalSchema: "nse",
                        principalTable: "CarrinhoClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Cliente",
                schema: "nse",
                table: "CarrinhoClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItems_CarrinhoId",
                schema: "nse",
                table: "CarrinhoItems",
                column: "CarrinhoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrinhoItems",
                schema: "nse");

            migrationBuilder.DropTable(
                name: "CarrinhoClientes",
                schema: "nse");
        }
    }
}
