using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRMMS.Migrations
{
    public partial class MigrationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cat_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.cat_id);
                });

            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    combo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    combo_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    combo_discription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    combo_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    combo_money = table.Column<decimal>(type: "money", nullable: true),
                    combo_status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combo", x => x.combo_id);
                });

            migrationBuilder.CreateTable(
                name: "Discount_code",
                columns: table => new
                {
                    code_id = table.Column<int>(type: "int", nullable: false),
                    code_detail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    discount_value = table.Column<double>(type: "float", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount_code", x => x.code_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    table_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: true),
                    Booking_id = table.Column<int>(type: "int", nullable: true),
                    TableOfPeople = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.table_id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    pro_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pro_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    pro_discription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pro_price = table.Column<decimal>(type: "money", nullable: true),
                    cat_id = table.Column<int>(type: "int", nullable: true),
                    pro_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pro_calories = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    pro_status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.pro_id);
                    table.ForeignKey(
                        name: "FK_Menu_Categories",
                        column: x => x.cat_id,
                        principalTable: "Categories",
                        principalColumn: "cat_id");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    acc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.acc_id);
                    table.ForeignKey(
                        name: "FK_Accounts_Role",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_date = table.Column<DateTime>(type: "date", nullable: true),
                    table_id = table.Column<int>(type: "int", nullable: true),
                    totalMoney = table.Column<decimal>(type: "money", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    code_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Order_Discount_code",
                        column: x => x.code_id,
                        principalTable: "Discount_code",
                        principalColumn: "code_id");
                    table.ForeignKey(
                        name: "FK_Order_Table",
                        column: x => x.table_id,
                        principalTable: "Table",
                        principalColumn: "table_id");
                });

            migrationBuilder.CreateTable(
                name: "StatusTable",
                columns: table => new
                {
                    Status_id = table.Column<int>(type: "int", nullable: false),
                    Status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTable", x => x.Status_id);
                    table.ForeignKey(
                        name: "FK_StatusTable_Table",
                        column: x => x.Status_id,
                        principalTable: "Table",
                        principalColumn: "table_id");
                });

            migrationBuilder.CreateTable(
                name: "Combo_Detail",
                columns: table => new
                {
                    combo_id = table.Column<int>(type: "int", nullable: false),
                    pro_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Combo Detail_Combo",
                        column: x => x.combo_id,
                        principalTable: "Combo",
                        principalColumn: "combo_id");
                    table.ForeignKey(
                        name: "FK_Combo Detail_Menu",
                        column: x => x.pro_id,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Booking_id = table.Column<int>(type: "int", nullable: false),
                    Time_booking = table.Column<DateTime>(type: "datetime", nullable: true),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: true),
                    acc_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Booking_id);
                    table.ForeignKey(
                        name: "FK_Booking_Accounts",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "acc_id");
                    table.ForeignKey(
                        name: "FK_Booking_Table",
                        column: x => x.Booking_id,
                        principalTable: "Table",
                        principalColumn: "table_id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false),
                    feedback = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    rate_star = table.Column<int>(type: "int", nullable: true),
                    acc_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Feedback_Accounts",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "acc_id");
                });

            migrationBuilder.CreateTable(
                name: "Point_List",
                columns: table => new
                {
                    point_id = table.Column<int>(type: "int", nullable: false),
                    acc_id = table.Column<int>(type: "int", nullable: true),
                    number_ponit = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point_List", x => x.point_id);
                    table.ForeignKey(
                        name: "FK_Point_List_Accounts",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "acc_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Details",
                columns: table => new
                {
                    order_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    pro_id = table.Column<int>(type: "int", nullable: true),
                    quantiity = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Details", x => x.order_detail_id);
                    table.ForeignKey(
                        name: "FK_Order_Details_Order",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_Order_Details_Products",
                        column: x => x.pro_id,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_role_id",
                table: "Accounts",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_acc_id",
                table: "Booking",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combo_Detail_combo_id",
                table: "Combo_Detail",
                column: "combo_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combo_Detail_pro_id",
                table: "Combo_Detail",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_acc_id",
                table: "Feedback",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_code_id",
                table: "Order",
                column: "code_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_table_id",
                table: "Order",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Details_order_id",
                table: "Order_Details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Details_pro_id",
                table: "Order_Details",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Point_List_acc_id",
                table: "Point_List",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_cat_id",
                table: "Products",
                column: "cat_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Combo_Detail");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Order_Details");

            migrationBuilder.DropTable(
                name: "Point_List");

            migrationBuilder.DropTable(
                name: "StatusTable");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Discount_code");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
