﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Storage.Internal;
using Recommend.Api.Data;
using System;

namespace Recommend.Api.Migrations
{
    [DbContext(typeof(RecommendContext))]
    partial class RecommendContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("Recommend.Api.Models.ProjectRecommend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Company");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("FinancingStage");

                    b.Property<string>("FromUserAvatar");

                    b.Property<int>("FromUserId");

                    b.Property<string>("FromUserName");

                    b.Property<string>("Introduction");

                    b.Property<string>("ProjectAvatar");

                    b.Property<int>("ProjectId");

                    b.Property<int>("RecommendType");

                    b.Property<DateTime>("RecommendedTime");

                    b.Property<string>("Tags");

                    b.Property<int>("UseId");

                    b.HasKey("Id");

                    b.ToTable("ProjectRecommends");
                });

            modelBuilder.Entity("Recommend.Api.Models.ProjectReferenceUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<string>("Company");

                    b.Property<int?>("ProjectRecommendId");

                    b.Property<int>("UseId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("ProjectRecommendId");

                    b.ToTable("ProjectReferenceUsers");
                });

            modelBuilder.Entity("Recommend.Api.Models.ProjectReferenceUser", b =>
                {
                    b.HasOne("Recommend.Api.Models.ProjectRecommend")
                        .WithMany("ReferenceUsers")
                        .HasForeignKey("ProjectRecommendId");
                });
#pragma warning restore 612, 618
        }
    }
}
