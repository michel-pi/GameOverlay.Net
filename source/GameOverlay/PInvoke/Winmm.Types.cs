using System;

namespace GameOverlay.PInvoke
{
	internal enum MultimediaResult : uint
	{
		NoError = 0,
		Error = 1,
		BadDeviceId = 2,
		NotEnabled = 3,
		Allocated = 4,
		InvalidHandle = 5,
		NoDriver = 6,
		NoMemory = 7,
		NotSupported = 8,
		BadErrorNumber = 9,
		InvalidFlag = 10,
		InvalidParameter = 11,
		HandleBusy = 12,
		InvalidAlias = 13,
		BadDatabase = 14,
		KeyNotFound = 15,
		ReadError = 16,
		WriteError = 17,
		DeleteError = 18,
		ValueNotFound = 19,
		NoDriverCB = 20,
		BadFormat = 32,
		StillPlaying = 33,
		Unprepared = 34
	}
}
