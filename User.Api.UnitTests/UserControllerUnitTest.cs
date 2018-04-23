using System;
using Xunit;
using User.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;

namespace User.Api.UnitTests
{
    public class UserControllerUnitTest
    {
        private Data.UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<Data.UserContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new Data.UserContext(options);
            context.Add(new Models.AppUser { Id = 1, Name = "check" });
            context.SaveChanges();
            return context;
        }

        private (UserController, Data.UserContext) GetUserController()
        {
            var context = this.GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            return (new UserController(context, loggerMoq.Object,null), context);
        }

        [Fact]
        public async Task Get_ReturnUser_WithUserId()
        {
            (UserController userController, Data.UserContext userContext) = this.GetUserController();
            var resp = await userController.Get();
            var result = resp.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appUser.Id.Should().Be(1);
            appUser.Name.Should().Be("check");
        }

        [Fact]
        public async Task Patch_ReturnNewName_WithParams()
        {
            (UserController userController, Data.UserContext userContext) = this.GetUserController();
            var doc = new JsonPatchDocument<Models.AppUser>();
            doc.Replace(u => u.Name, "che");
            var resp = await userController.Patch(doc);
            var result = resp.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appUser.Name.Should().Be("che");

            var user = await userContext.Users.FirstOrDefaultAsync(u => u.Id == 1);
            user.Should().NotBeNull();
            user.Name.Should().Be("che");
        }


        [Fact]
        public async Task Patch_ReturnNewProperties_WithNewProperty()
        {
            (UserController userController, Data.UserContext userContext) = this.GetUserController();
            var doc = new JsonPatchDocument<Models.AppUser>();
            doc.Replace(u => u.Properties, new List<Models.UserProperty>
            { new Models.UserProperty
            {
                Key ="fin_industry",
                Value="Mongo",
                Text ="Mongo"
            }
            });
            var resp = await userController.Patch(doc);
            var result = resp.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appUser.Properties.Count.Should().Be(1);
            appUser.Properties.First().Value.Should().Be("Mongo");
            appUser.Properties.First().Key.Should().Be("fin_industry");

            var user = await userContext.Users.FirstOrDefaultAsync(u => u.Id == 1);
            user.Properties.Count.Should().Be(1);
            user.Properties.First().Value.Should().Be("Mongo");
            user.Properties.First().Key.Should().Be("fin_industry");
        }



        [Fact]
        public async Task Patch_ReturnNewProperties_WithRemvoeProperty()
        {
            (UserController userController, Data.UserContext userContext) = this.GetUserController();
            var doc = new JsonPatchDocument<Models.AppUser>();
            doc.Replace(u => u.Properties, new List<Models.UserProperty>());
            var resp = await userController.Patch(doc);
            var result = resp.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appUser.Properties.Count.Should().Be(0);

            var user = await userContext.Users.FirstOrDefaultAsync(u => u.Id == 1);
            user.Properties.Should().BeEmpty();
        }
    }
}
