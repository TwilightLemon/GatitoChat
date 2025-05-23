﻿using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

[JsonSerializable(typeof(MessageEntity))]
[JsonSerializable(typeof(MessageResponse))]
[JsonSerializable(typeof(CheckResponse))]
[JsonSerializable(typeof(VerifierEntity))]
[JsonSerializable(typeof(VerifierResponse))]
[JsonSerializable(typeof(ExitEntity))]
[JsonSerializable(typeof(RegisterEntity))]
[JsonSerializable(typeof(LoginEntity))]
[JsonSerializable(typeof(RegisterEntity))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSourceGenerationOptions]
internal partial class AppJsonContext : JsonSerializerContext
{
}