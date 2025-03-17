using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace onlineshop.Migrations
{
    /// <inheritdoc />
    public partial class adding_temps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_L_BasketItem");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "T_Basket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "T_TempOrder",
                columns: table => new
                {
                    ID_TempOrder = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    T_User_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TempOrder", x => x.ID_TempOrder);
                });

            migrationBuilder.CreateTable(
                name: "T_TempBasket",
                columns: table => new
                {
                    ID_TempBasket = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    T_Product_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<long>(type: "bigint", nullable: false),
                    T_tempOrder_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TempBasket", x => x.ID_TempBasket);
                    table.ForeignKey(
                        name: "FK_T_TempBasket_T_Product_T_Product_ID",
                        column: x => x.T_Product_ID,
                        principalTable: "T_Product",
                        principalColumn: "ID_Product",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_TempBasket_T_TempOrder_T_tempOrder_ID",
                        column: x => x.T_tempOrder_ID,
                        principalTable: "T_TempOrder",
                        principalColumn: "ID_TempOrder",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_TempBasket_T_Product_ID",
                table: "T_TempBasket",
                column: "T_Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_TempBasket_T_tempOrder_ID",
                table: "T_TempBasket",
                column: "T_tempOrder_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TempBasket");

            migrationBuilder.DropTable(
                name: "T_TempOrder");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "T_Basket");

            migrationBuilder.CreateTable(
                name: "T_L_BasketItem",
                columns: table => new
                {
                    ID_BasketItem = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    T_Basket_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    T_Product_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_L_BasketItem", x => x.ID_BasketItem);
                    table.ForeignKey(
                        name: "FK_T_L_BasketItem_T_Basket_T_Basket_ID",
                        column: x => x.T_Basket_ID,
                        principalTable: "T_Basket",
                        principalColumn: "ID_Basket",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_L_BasketItem_T_Product_T_Product_ID",
                        column: x => x.T_Product_ID,
                        principalTable: "T_Product",
                        principalColumn: "ID_Product",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_L_BasketItem_T_Basket_ID",
                table: "T_L_BasketItem",
                column: "T_Basket_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_L_BasketItem_T_Product_ID",
                table: "T_L_BasketItem",
                column: "T_Product_ID");
        }
    }
}
