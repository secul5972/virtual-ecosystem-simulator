using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimalContents : MonoBehaviour
{
	public Dropdown AnimalDropDown;
	public InputField CntInputField;
	public GameObject animalInfo;

	private Dictionary<string, int> animalSelection = new Dictionary<string, int>();
	public Dictionary<string, int> AnimalSelection
    {
        get { return animalSelection; }
    }

	private void Start()
	{
		foreach(var animal in AnimalList.animalList)
		{
			animalSelection.Add(animal.Key, 0);
			AnimalDropDown.options.Add(new Dropdown.OptionData(animal.Key));
		}
	}

	public void OnSelectClick()
	{
		if (CntInputField.textComponent.text.Length == 0 || int.Parse(CntInputField.textComponent.text) == 0)
			return;
		GameObject animalContent = Instantiate(animalInfo);
		animalContent.transform.position = new Vector3(920, -120, 0);
		animalContent.transform.SetParent(transform);

		Text name = animalContent.transform.Find("Name").gameObject.GetComponent<Text>();
		name.text = AnimalDropDown.captionText.text;

		Text count = animalContent.transform.Find("Count").gameObject.GetComponent<Text>();
		count.text = int.Parse(CntInputField.textComponent.text).ToString();

		animalSelection[name.text] = int.Parse(count.text);

		Button remove =	animalContent.transform.transform.Find("Remove").GetComponent<Button>();
		remove.onClick.AddListener(OnRemoveClick);

		animalContent.SetActive(true);
	}

	public void OnRemoveClick()
	{
		Destroy(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);
	}

	public void UploadAnimalData()
	{
		foreach(var animal in animalSelection)
        {
			PlayerPrefs.SetInt(animal.Key, animal.Value); //동물의 이름과, 해당하는 개체수를 PlayerPrefs 사용해서 업로드.
        }
	}
}
