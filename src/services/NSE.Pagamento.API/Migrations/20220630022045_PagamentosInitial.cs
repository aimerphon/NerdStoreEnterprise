using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Pagamentos.API.Migrations
{
    public partial class PagamentosInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nse");

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                schema: "nse",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PedidoId = table.Column<Guid>(nullable: false),
                    TipoPagamento = table.Column<int>(nullable: false),
                    Valor = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                schema: "nse",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CodigoAutorizacao = table.Column<string>(type: "varchar(100)", nullable: true),
                    BandeiraCartao = table.Column<string>(type: "varchar(100)", nullable: true),
                    DataTransacao = table.Column<DateTime>(nullable: true),
                    ValorTotal = table.Column<decimal>(nullable: false),
                    CustoTransacao = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TID = table.Column<string>(type: "varchar(100)", nullable: true),
                    NSU = table.Column<string>(type: "varchar(100)", nullable: true),
                    PagamentoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Pagamentos_PagamentoId",
                        column: x => x.PagamentoId,
                        principalSchema: "nse",
                        principalTable: "Pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_PagamentoId",
                schema: "nse",
                table: "Transacoes",
                column: "PagamentoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transacoes",
                schema: "nse");

            migrationBuilder.DropTable(
                name: "Pagamentos",
                schema: "nse");
        }
    }
}
