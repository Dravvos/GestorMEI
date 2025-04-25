using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorMEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstoqueMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomeFantasia",
                table: "Empresa",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<float>(type: "real", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantidadeEstoque = table.Column<long>(type: "bigint", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdTGTipoLancamento = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<long>(type: "bigint", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "text", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAlteracao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoque_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estoque_EmpresaId",
                table: "Estoque",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoque_ProdutoId",
                table: "Estoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_EmpresaId",
                table: "Produtos",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estoque");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.AlterColumn<string>(
                name: "NomeFantasia",
                table: "Empresa",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
