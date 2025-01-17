﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShippingService.Database;

#nullable disable

namespace ShippingService.Database.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    partial class ShippingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shipping-fast-endpoints")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShippingService.Domain.Customers.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.HasKey("Id")
                        .HasName("pk_customers");

                    b.ToTable("customers", "shipping-fast-endpoints");
                });

            modelBuilder.Entity("ShippingService.Domain.Orders.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customer_id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("order_number");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("CustomerId")
                        .HasDatabaseName("ix_orders_customer_id");

                    b.HasIndex("OrderNumber")
                        .HasDatabaseName("ix_orders_order_number");

                    b.ToTable("orders", "shipping-fast-endpoints");
                });

            modelBuilder.Entity("ShippingService.Domain.Orders.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("product");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("pk_order_item");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_order_item_order_id");

                    b.ToTable("order_item", "shipping-fast-endpoints");
                });

            modelBuilder.Entity("ShippingService.Domain.Shipments.Shipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Carrier")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("carrier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("number");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<string>("ReceiverEmail")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("receiver_email");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_shipments");

                    b.HasIndex("Number")
                        .HasDatabaseName("ix_shipments_number");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_shipments_order_id");

                    b.ToTable("shipments", "shipping-fast-endpoints");
                });

            modelBuilder.Entity("ShippingService.Domain.Shipments.ShipmentItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("product");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.Property<Guid>("ShipmentId")
                        .HasColumnType("uuid")
                        .HasColumnName("shipment_id");

                    b.HasKey("Id")
                        .HasName("pk_shipment_items");

                    b.HasIndex("ShipmentId")
                        .HasDatabaseName("ix_shipment_items_shipment_id");

                    b.ToTable("shipment_items", "shipping-fast-endpoints");
                });

            modelBuilder.Entity("ShippingService.Domain.Orders.Order", b =>
                {
                    b.HasOne("ShippingService.Domain.Customers.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_orders_customers_customer_id");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("ShippingService.Domain.Orders.OrderItem", b =>
                {
                    b.HasOne("ShippingService.Domain.Orders.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_item_orders_order_id");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("ShippingService.Domain.Shipments.Shipment", b =>
                {
                    b.HasOne("ShippingService.Domain.Orders.Order", null)
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shipments_orders_order_id");

                    b.OwnsOne("ShippingService.Domain.Shipments.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("ShipmentId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("address_city");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("address_street");

                            b1.Property<string>("Zip")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("address_zip");

                            b1.HasKey("ShipmentId");

                            b1.ToTable("shipments", "shipping-fast-endpoints");

                            b1.WithOwner()
                                .HasForeignKey("ShipmentId")
                                .HasConstraintName("fk_shipments_shipments_id");
                        });

                    b.Navigation("Address")
                        .IsRequired();
                });

            modelBuilder.Entity("ShippingService.Domain.Shipments.ShipmentItem", b =>
                {
                    b.HasOne("ShippingService.Domain.Shipments.Shipment", "Shipment")
                        .WithMany("Items")
                        .HasForeignKey("ShipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shipment_items_shipments_shipment_id");

                    b.Navigation("Shipment");
                });

            modelBuilder.Entity("ShippingService.Domain.Customers.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("ShippingService.Domain.Orders.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("ShippingService.Domain.Shipments.Shipment", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
