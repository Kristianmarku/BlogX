using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogX.Migrations
{
    public partial class AddedUserRoleRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserModelId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserModelId",
                table: "AspNetUserRoles",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserModelId",
                table: "AspNetUserRoles",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserModelId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserModelId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "AspNetUserRoles");
        }
    }
}
