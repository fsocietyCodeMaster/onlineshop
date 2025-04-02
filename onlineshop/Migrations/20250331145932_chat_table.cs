using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace onlineshop.Migrations
{
    /// <inheritdoc />
    public partial class chat_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "AssignedAdminId",
                table: "T_ChatConnection",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AssignedAdminId",
                table: "T_ChatConnection");



        }
    }
}
