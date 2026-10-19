using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModEntryBehaviour : MonoBehaviour, ISelectHandler, IEventSystemHandler, IPointerDownHandler
{
	public ModListBehaviour ModListBehaviour;

	public Button MainButton;

	public GameObject ActiveCheck;

	public GameObject InactiveCheck;

	public GameObject ErrorCheck;

	public GameObject CompilingCheck;

	public GameObject SuspiciousCheck;

	public GameObject RecompileButton;

	public GameObject ToggleModButton;

	public GameObject UploadButton;

	public GameObject TrustButton;

	public GameObject OpenInWorkshopButton;

	public TextMeshProUGUI Title;

	public TextMeshProUGUI Subtitle;

	public TextMeshProUGUI Description;

	public Image Thumbnail;

	[Space]
	public ModMetaData ModMeta;

	private float lastClickTime = float.NegativeInfinity;

	private bool isBusy;

	private void Start()
	{
		UpdateUi();
	}

	public void ShowDescription()
	{
		ModListBehaviour.ModDescription.text = "<size=1.5em>" + ModMeta.Name + "</size>\n" + (ModMeta.RichReadme ?? ((ModMeta.Description ?? string.Empty) + "\n<noparse>A mod by " + ModMeta.Author + " for game version " + ModMeta.GameVersion + "</noparse>"));
	}

	public void UpdateUi()
	{
		if (ModMeta == null)
		{
			return;
		}
		Title.text = Utils.EscapeRichText(ModMeta.Name);
		Subtitle.text = "by <b>" + Utils.EscapeRichText(ModMeta.Author) + "</b> for " + Utils.EscapeRichText(ModMeta.GameVersion);
		Sprite thumbnail = ModLoader.GetThumbnail(ModMeta);
		if (thumbnail != null)
		{
			Thumbnail.sprite = thumbnail;
		}
		ActiveCheck.SetActive(ModMeta.Active && !ModMeta.HasErrors);
		InactiveCheck.SetActive(!ModMeta.Active && !ModMeta.HasErrors);
		ErrorCheck.SetActive(ModMeta.HasErrors && !ModMeta.Suspicious);
		SuspiciousCheck.SetActive(ModMeta.HasErrors && ModMeta.Suspicious);
		ToggleModButton.SetActive(!ModMeta.HasErrors);
		RecompileButton.SetActive(ModMeta.HasErrors);
		TrustButton.SetActive(ModMeta.HasErrors && ModMeta.Suspicious);
		CompilingCheck.SetActive(value: false);
		OpenInWorkshopButton.SetActive(ModMeta.UGCIdentity != null || ModMeta.CreatorUGCIdentity != null);
		if (!string.IsNullOrWhiteSpace(ModMeta.Errors) && ModMeta.HasErrors)
		{
			if (ModMeta.Suspicious)
			{
				Description.color = Color.yellow;
				Description.text = "This mod contains potentially harmful code. You may accept this risk, but you might regret it.\n" + string.Join("\n", ModMeta.Errors);
			}
			else
			{
				Description.color = Color.red;
				Description.text = string.Join("\n", ModMeta.Errors);
			}
		}
		else
		{
			Description.color = Color.white;
			Description.text = ModMeta.Description;
		}
		UploadButton.SetActive(!ModMeta.HasErrors && string.IsNullOrEmpty(ModMeta.UGCIdentity) && !ModMeta.HasDoNotReuploadSign());
	}

	public void ToggleMod()
	{
		if (!isBusy)
		{
			ModLoader.SetModActive(ModMeta, !ModMeta.Active);
			UpdateUi();
		}
	}

	public void TrustMod()
	{
		TrustCache.AssertCaller("trust mod " + ModMeta.GetUniqueName() + " via the ModEntryBehaviour");
		if (!isBusy && !DialogBox.IsAnyDialogboxOpen)
		{
			DialogBoxManager.Dialog("<b>Are you sure?</b>\r\nThis mod could seriously harm your computer. It could also be fine.\r\nDo you want to continue? This cannot be undone.", new DialogButton("Cancel", true), new DialogButton("Continue", true, delegate
			{
				TrustCache.Trust(ModMeta);
				TrustCache.Save();
				ForceReload();
			}));
		}
	}

	public void ForceReload()
	{
		if (!isBusy)
		{
			ActiveCheck.SetActive(value: false);
			InactiveCheck.SetActive(value: false);
			SuspiciousCheck.SetActive(value: false);
			ErrorCheck.SetActive(value: false);
			ToggleModButton.SetActive(value: false);
			RecompileButton.SetActive(value: false);
			TrustButton.SetActive(value: false);
			CompilingCheck.SetActive(value: true);
			StartCoroutine(Recompile());
		}
	}

	public void OpenInWorkshop()
	{
		string text = ModMeta.UGCIdentity ?? ModMeta.CreatorUGCIdentity;
		if (text != null)
		{
			Utils.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + text);
		}
	}

	public void UploadMod()
	{
		if (!isBusy)
		{
			DialogBoxManager.Dialog("Do you want to upload \"" + ModMeta.Name + "\" to the workshop?", new DialogButton("Yes", true, delegate
			{
				uploadMod();
			}), new DialogButton("No", true));
		}
		void uploadMod()
		{
			try
			{
				ModLoader.UploadMod(ModMeta);
			}
			catch (Exception ex)
			{
				UISoundBehaviour.Main.Error();
				DialogBoxManager.Notification("Unable to upload mod\n" + ex.Message);
			}
		}
	}

	private IEnumerator Recompile()
	{
		if (!isBusy)
		{
			isBusy = true;
			Description.text = string.Empty;
			BackgroundItemLoader background = BackgroundItemLoader.Instance;
			yield return new WaitForSeconds(0.1f);
			(ModCompilationResult result, ModScript script) result = default((ModCompilationResult, ModScript));
			yield return background.DoAsyncTask(async delegate
			{
				result = await background.CompileMod(ModMeta);
			}, ModMeta.Name, "Processing", ModLoader.GetThumbnail(ModMeta));
			background.ProcessModCompilationResult(result, ModMeta);
			yield return new WaitForSeconds(0.1f);
			UpdateUi();
			if (ModMeta == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			isBusy = false;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		ShowDescription();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (Mathf.Abs(lastClickTime - Time.unscaledTime) < 0.3f)
		{
			ToggleMod();
		}
		lastClickTime = Time.unscaledTime;
	}
}
