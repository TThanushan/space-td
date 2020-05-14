using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorNode : MonoBehaviour
{
    private GameObject currentPrefab;
    private bool firstTimeReplacing = true;

    private void OnMouseEnter()
    {
        CheckIfPrefabIsChanged();
        UpdateCursorPosition();
        EnableCursor();
    }

    private void OnMouseOver()
    {
        CheckIfPrefabIsChanged();
    }

    private void OnMouseExit()
    {
        DisableCursor();
        firstTimeReplacing = true;
    }

    public void SetCurrentPrefab(GameObject obj)
    {
        currentPrefab = obj;
    }

    public GameObject GetCurrentPrefab()
    {
        return currentPrefab;
    }

    private void UpdateCursorPosition()
    {
        LevelEditor.instance.cursor.transform.position = transform.position;
    }

    private void EnableCursor()
    {
        LevelEditor.instance.cursor.SetActive(true);
    }

    private void DisableCursor()
    {
        LevelEditor.instance.cursor.SetActive(false);
    }

    private void CheckIfPrefabIsChanged()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (IsNewPrefabPlaced())
                CreatePrefab();
            else if (IsOldPrefabReplaced())
                ReplacePrefab();
        }
        else if (KeyPressManager.instance.mouse1KeyReady)
            DestroyPrefab();
    }
    
    private bool IsNewPrefabPlaced()
    {
        return !currentPrefab && IsPrefabSelected();
    }

    private bool IsOldPrefabReplaced()
    {
        return currentPrefab && IsPrefabSelected();
    }
 
    private bool IsPrefabSelected()
    {
        return LevelEditor.instance.selectedPrefab != null;
    }

    private void InstantiatePrefab()
    {
        GameObject selectedPrefab = LevelEditor.instance.selectedPrefab;
        currentPrefab = PoolObject.instance.GetPoolObject(selectedPrefab);
        currentPrefab.transform.position = transform.position;
        selectedPrefab.SetActive(true);
    }

    private void RemoveCurrentPrefab()
    {
        currentPrefab.SetActive(false);
        currentPrefab = null;
    }

    private bool PrefabAlreadyCreatedOnCurrentNode()
    {
        return currentPrefab && currentPrefab.name == LevelEditor.instance.selectedPrefab.name;
    }
    private void CreatePrefab()
    {
        if (currentPrefab || PrefabAlreadyCreatedOnCurrentNode())
            return;
        Debug.Log("Prefab created");
        InstantiatePrefab();
        LevelEditorUI.instance.CreatePlacingEffect(transform.position);
        AudioManager.instance.PlaySfx("Click");
    }

    private void ReplacePrefab()
    {
        if (!firstTimeReplacing || PrefabAlreadyCreatedOnCurrentNode())
            return;
        Debug.Log("Prefab replaced");
        DestroyPrefab();
        CreatePrefab();
        firstTimeReplacing = false;
    }

    private void DestroyPrefab()
    {
        if (!currentPrefab)
            return;
        Debug.Log("Prefab destroyed");
        RemoveCurrentPrefab();
        LevelEditorUI.instance.CreateDeleteEffect(transform.position);
        AudioManager.instance.PlaySfx("Erase");
    }

    public void ClearCurrentPrefab()
    {
        if (!currentPrefab)
            return;
        currentPrefab.SetActive(false);
        currentPrefab = null;
    }

}
