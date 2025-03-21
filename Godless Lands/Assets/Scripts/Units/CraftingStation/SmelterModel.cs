﻿using Inventory;
using Items;
using Protocol.Data.Units.CraftingStation;
using Recipes;
using Recipes.Data;
using System;
using System.Collections.Generic;

namespace CraftingStations
{
    public class SmelterModel : IDisposable
    {
        private Item[] _components = new Item[SmelterConfig.COMPONENT_SIZE];
        private Item[] _fuels = new Item[SmelterConfig.FUEL_SIZE];
        private List<RecipeData> _recipes = new List<RecipeData>();
        private RecipeComponentMatcherService _recipeComponentMatcherService;
        private CraftingStationType _smelterUse;
        private InventoryModel _inventoryModel;

        public IReadOnlyCollection<Item> Components => _components;
        public IReadOnlyCollection<Item> Fuels => _fuels;
        public IReadOnlyCollection<RecipeData> Recipes => _recipes;

        public event Action OnContentUpdate;

        public SmelterModel(RecipeComponentMatcherService recipeComponentMatcherService, InventoryModel inventoryModel)
        {
            _recipeComponentMatcherService = recipeComponentMatcherService;
            _inventoryModel = inventoryModel;
            _inventoryModel.OnInventoryUpdated += OnInventoryUpdate;
        }

        private void OnInventoryUpdate()
        {
            UpdateItems(_components);
            UpdateItems(_fuels);
            OnContentUpdate?.Invoke();
        }

        private void UpdateItems(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    continue;
                SetItem(items, i, _inventoryModel.FindItem(items[i].UniqueID));
            }
        }

        public void AddComponent(int index, Item item)
        {
            if (index < 0 || index >= SmelterConfig.COMPONENT_SIZE)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            RemoveExistingItem(_components, item);

            SetItem(_components, index, item);

            _recipes = _recipeComponentMatcherService.FindRecipe(_components, _fuels, _smelterUse);

            OnContentUpdate?.Invoke();
        }

        public void AddFuel(int index, Item item)
        {
            if (index < 0 || index >= SmelterConfig.FUEL_SIZE)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            RemoveExistingItem(_fuels, item);

            SetItem(_fuels, index, item);

            _recipes = _recipeComponentMatcherService.FindRecipe(Components, Fuels, _smelterUse);

            OnContentUpdate?.Invoke();
        }

        private void RemoveExistingItem(Item[] items, Item item)
        {
            if(item == null)
            {
                return;
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].UniqueID == item.UniqueID)
                {
                    SetItem(items, i, null);
                }
            }
        }

        private void SetItem(Item[] items, int index, Item item)
        {
            if (items[index] != null)
                _inventoryModel.UnlockItem(items[index].UniqueID);

            items[index] = item;

            if (items[index] != null)
                _inventoryModel.LockItem(items[index].UniqueID);
        }

        public void ReserverSmelterModel(CraftingStationType workbenchTypeUse)
        {
            _smelterUse = workbenchTypeUse;
        }

        internal void ReleaseSmelterModel()
        {
            for (int i = 0; i < _components.Length; i++)
            {
                SetItem(_components, i, null);
            }
            for (int i = 0; i < _fuels.Length; i++)
            {
                SetItem(_fuels, i, null);
            }
        }

        public void Dispose()
        {
            _inventoryModel.OnInventoryUpdated -= OnInventoryUpdate;
        }

      
    }
}

