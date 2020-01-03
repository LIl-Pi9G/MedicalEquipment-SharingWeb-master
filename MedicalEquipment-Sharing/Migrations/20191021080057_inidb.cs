using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedicalEquipment_Sharing.Migrations
{
    public partial class inidb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalEquipments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Describe = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    UserEmail = table.Column<string>(nullable: false),
                    SoldPrice = table.Column<decimal>(nullable: false),
                    OriginalPrice = table.Column<decimal>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Manufacturer = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalEquipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EquipmentId = table.Column<long>(nullable: false),
                    SellerEmail = table.Column<string>(nullable: false),
                    BuyerEmail = table.Column<string>(nullable: false),
                    SoldDate = table.Column<DateTime>(nullable: false),
                    RevertedDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    PayOrderNum = table.Column<string>(nullable: true),
                    DepositOrderNum = table.Column<string>(nullable: true),
                    PaySuccess = table.Column<bool>(nullable: false),
                    PayDepositSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "PayOrders",
                columns: table => new
                {
                    OrderNum = table.Column<string>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    PaySuccess = table.Column<bool>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Account = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOrders", x => x.OrderNum);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    TelePhone = table.Column<string>(maxLength: 30, nullable: false),
                    Income = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalEquipments");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PayOrders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
