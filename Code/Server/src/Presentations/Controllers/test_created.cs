using System;
using Microsoft.AspNetCore.Mvc;

public class MyController : ControllerBase {
    public IActionResult Test() {
        return Created();
    }
}
