using UnityEngine;
using System.Collections;

public static class CAT_CopyUtils
{
	static CAT_Action copiedAction;
	static CAT_Event copiedEvent;

	public static void CopyAction(CAT_Action original)
	{
		copiedAction = original.InternalCopy();
	}

	public static CAT_Action PasteAction()
	{
		CAT_Action ret = copiedAction.InternalCopy();
		return ret;
	}

	public static bool HasACopiedAction()
	{
		return (copiedAction != null);
	}


	public static void CopyEvent(CAT_Event original)
	{
		copiedEvent = original.CopyEventContent();
	}

	public static CAT_Event PasteEvent()
	{
		CAT_Event ev = copiedEvent.CopyEventContent();
		ev.userFriendlyName = "COPY OF " + ev.userFriendlyName;
		return ev;
	}

	public static bool HasACopiedEvent()
	{
		return (copiedEvent != null);
	}
}
