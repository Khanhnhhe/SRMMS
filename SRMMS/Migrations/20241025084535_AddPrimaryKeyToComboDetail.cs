using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRMMS.Migrations
{
    public partial class AddPrimaryKeyToComboDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    cat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cat_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    combo_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    combo_discription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    combo_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    combo_money = table.Column<decimal>(type: "money", nullable: false),
                    combo_status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combo", x => x.combo_id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    cus_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cus_fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cus_phone = table.Column<int>(type: "int", nullable: false),
                    CusPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ponit = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.cus_id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurant_Information",
                columns: table => new
                {
                    res_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    res_adress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    res_facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    res_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    res_phone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table_id = table.Column<int>(type: "int", nullable: false),
                    table_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QR_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
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
                    pro_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    pro_discription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pro_price = table.Column<decimal>(type: "money", nullable: false),
                    cat_id = table.Column<int>(type: "int", nullable: false),
                    pro_img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pro_calories = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    pro_cooking_time = table.Column<int>(type: "int", nullable: false),
                    pro_status = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Employees",
                columns: table => new
                {
                    emp_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emp_first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    emp_last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    emp_dob = table.Column<DateTime>(type: "date", nullable: true),
                    emp_gender = table.Column<bool>(type: "bit", nullable: false),
                    emp_phoneNumber = table.Column<int>(type: "int", nullable: false),
                    emp_email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    emp_password = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    emp_address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    emp_ward = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    emp_startDate = table.Column<DateTime>(type: "date", nullable: false),
                    emp_endDate = table.Column<DateTime>(type: "date", nullable: true),
                    emp_role_id = table.Column<int>(type: "int", nullable: false),
                    emp_status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.emp_id);
                    table.ForeignKey(
                        name: "FK_Employees_Role",
                        column: x => x.emp_role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Combo_Detail",
                columns: table => new
                {
                    combo_id = table.Column<int>(type: "int", nullable: false),
                    pro_id = table.Column<int>(type: "int", nullable: false)
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
                name: "Accounts",
                columns: table => new
                {
                    acc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cus_id = table.Column<int>(type: "int", nullable: false),
                    emp_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.acc_id);
                    table.ForeignKey(
                        name: "FK_Accounts_Customers",
                        column: x => x.cus_id,
                        principalTable: "Customers",
                        principalColumn: "cus_id");
                    table.ForeignKey(
                        name: "FK_Accounts_Employees",
                        column: x => x.emp_id,
                        principalTable: "Employees",
                        principalColumn: "emp_id");
                });

            migrationBuilder.CreateTable(
                name: "Booking_table",
                columns: table => new
                {
                    book_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    table_id = table.Column<int>(type: "int", nullable: true),
                    acc_id = table.Column<int>(type: "int", nullable: true),
                    time_booking = table.Column<DateTime>(type: "date", nullable: true),
                    time_out = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking_table", x => x.book_id);
                    table.ForeignKey(
                        name: "FK_Booking_table_Accounts",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "acc_id");
                    table.ForeignKey(
                        name: "FK_Booking_table_Table",
                        column: x => x.table_id,
                        principalTable: "Table",
                        principalColumn: "table_id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    pro_id = table.Column<int>(type: "int", nullable: false),
                    feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ratestar = table.Column<int>(type: "int", nullable: false),
                    acc_id = table.Column<int>(type: "int", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Feedback_Menu",
                        column: x => x.pro_id,
                        principalTable: "Products",
                        principalColumn: "pro_id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_date = table.Column<DateTime>(type: "date", nullable: false),
                    acc_id = table.Column<int>(type: "int", nullable: false),
                    totalMoney = table.Column<decimal>(type: "money", nullable: false),
                    order_status_id = table.Column<int>(type: "int", nullable: false),
                    staus = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Order_Accounts",
                        column: x => x.acc_id,
                        principalTable: "Accounts",
                        principalColumn: "acc_id");
                });

            migrationBuilder.CreateTable(
                name: "Discount_code",
                columns: table => new
                {
                    code_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    code_detail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    discount_value = table.Column<double>(type: "float", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount_code", x => x.code_id);
                    table.ForeignKey(
                        name: "FK_Discount_code_Order",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Details",
                columns: table => new
                {
                    order_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    pro_id = table.Column<int>(type: "int", nullable: true),
                    quantiity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false)
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
                name: "IX_Accounts_cus_id",
                table: "Accounts",
                column: "cus_id");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_emp_id",
                table: "Accounts",
                column: "emp_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_table_acc_id",
                table: "Booking_table",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_table_table_id",
                table: "Booking_table",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combo_Detail_combo_id",
                table: "Combo_Detail",
                column: "combo_id");

            migrationBuilder.CreateIndex(
                name: "IX_Combo_Detail_pro_id",
                table: "Combo_Detail",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_code_order_id",
                table: "Discount_code",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_emp_role_id",
                table: "Employees",
                column: "emp_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_acc_id",
                table: "Feedback",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_pro_id",
                table: "Feedback",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_acc_id",
                table: "Order",
                column: "acc_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Details_order_id",
                table: "Order_Details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Details_pro_id",
                table: "Order_Details",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_cat_id",
                table: "Products",
                column: "cat_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking_table");

            migrationBuilder.DropTable(
                name: "Combo_Detail");

            migrationBuilder.DropTable(
                name: "Discount_code");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Order_Details");

            migrationBuilder.DropTable(
                name: "Restaurant_Information");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
