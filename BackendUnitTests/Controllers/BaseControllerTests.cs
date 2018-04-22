using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BackendUnitTests.Controllers
{
    public abstract class BaseControllerTests<T> where T : Controller
    {
        protected T Controller;

        protected abstract void OnSetup();

        [SetUp]
        public void Setup()
        {
            OnSetup();
            Assert.NotNull(Controller, "Controller was not initialized");
        }
    }
}
