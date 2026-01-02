using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instanceText;
    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private Button sellBtn;

    private string playersInventoryItemId;
    private Func<int> getPrice;
    private Func<string, int, Task> createListingAsync;

    public void Bind(
        PlayersInventoryItem item,
        Func<int> getSellPrice,
        Func<string, int, Task> createListingFunc)
    {
        playersInventoryItemId = item.PlayersInventoryItemId;
        getPrice = getSellPrice;
        createListingAsync = createListingFunc;

        if (titleText != null) titleText.text = item.InventoryItemId;

        string shortInstance = !string.IsNullOrEmpty(playersInventoryItemId) && playersInventoryItemId.Length > 8
            ? playersInventoryItemId.Substring(0, 8)
            : playersInventoryItemId;

        if (instanceText != null) instanceText.text = $"instance: {shortInstance}";

        if (optionText != null) optionText.text = "option: -";

        if (sellBtn != null)
        {
            sellBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.AddListener(() => { _ = SellAsync(); });
        }
    }

    private async Task SellAsync()
    {
        if (createListingAsync == null) return;

        int price = getPrice != null ? getPrice.Invoke() : 100;
        await createListingAsync.Invoke(playersInventoryItemId, price);
    }

    private static string TryGetString(Dictionary<string, object> data, string key, string defaultValue)
    {
        if (data == null) return defaultValue;
        if (!data.TryGetValue(key, out object value)) return defaultValue;
        return value != null ? value.ToString() : defaultValue;
    }

    private static int TryGetInt(Dictionary<string, object> data, string key, int defaultValue)
    {
        if (data == null) return defaultValue;
        if (!data.TryGetValue(key, out object value)) return defaultValue;

        if (value is int i) return i;
        if (value is long l) return (int)l;
        if (int.TryParse(value.ToString(), out int parsed)) return parsed;

        return defaultValue;
    }
}
