﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reductech.EDR.ConnectorManagement.Base;
using Xunit;

namespace Reductech.EDR.ConnectorManagement.Tests
{

public class ConnectorSettingsTests
{
    [Fact]
    public void DefaultForAssembly_ByDefault_ReturnsAssemblyInformation()
    {
        const string name    = "test";
        const string version = "0.9.0";

        var assembly = new Mock<Assembly>();
        assembly.Setup(a => a.GetName()).Returns(() => new AssemblyName(name));

        assembly.Setup(a => a.GetCustomAttributes(It.IsAny<Type>(), true))
            .Returns(() => new Attribute[] { new AssemblyInformationalVersionAttribute(version) });

        var settings = ConnectorSettings.DefaultForAssembly(assembly.Object);

        Assert.Equal(name,    settings.Id);
        Assert.Equal(version, settings.Version);
        Assert.True(settings.Enable);
    }

    [Fact]
    public void DefaultForAssembly_WhenNameAndVersionAreNull_ReturnsUnknown()
    {
        const string unknown = "Unknown";

        var assembly = new Mock<Assembly>();
        assembly.Setup(a => a.GetName()).Returns(() => new AssemblyName());

        assembly.Setup(a => a.GetCustomAttributes(It.IsAny<Type>(), true))
            .Returns(() => Array.Empty<Attribute>());

        var settings = ConnectorSettings.DefaultForAssembly(assembly.Object);

        Assert.Equal(unknown, settings.Id);
        Assert.Equal(unknown, settings.Version);
    }

    [Fact]
    public void Settings_IsCorrectlyDeserialized()
    {
        var expectedFeatures = new[] { "ANALYSIS", "CASE_CREATION" };

        var settings =
            JsonConvert.DeserializeObject<Dictionary<string, ConnectorSettings>>(
                Helpers.TestConfiguration
            );

        var nuixSettings = settings["Reductech.EDR.Connectors.Nuix"];

        Assert.NotNull(nuixSettings.Settings);
        Assert.Equal("dongle", nuixSettings.Settings!["licencesourcetype"]);

        var nuixFeatures = nuixSettings.Settings!["features"] as JArray;

        Assert.NotNull(nuixFeatures);
        Assert.Equal(expectedFeatures, nuixFeatures!.ToObject<string[]>());
    }
}

}