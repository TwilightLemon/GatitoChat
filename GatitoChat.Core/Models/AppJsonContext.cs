﻿using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

[JsonSerializable(typeof(MessageEntity))]
[JsonSerializable(typeof(MessageResponse))]
[JsonSerializable(typeof(ExitEntity))]
[JsonSerializable(typeof(RegisterEntity))]
[JsonSerializable(typeof(LoginEntity))]
[JsonSerializable(typeof(RegisterEntity))]
[JsonSerializable(typeof(VerificationEntity))]
[JsonSerializable(typeof(ActionResponse))]
[JsonSerializable(typeof(RoomManageEntity))]
[JsonSourceGenerationOptions]
internal partial class AppJsonContext : JsonSerializerContext
{
}