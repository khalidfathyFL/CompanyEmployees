using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CompanyEmployees.Presentation.ModelBinders
{
  public class ArrayModelBinder : IModelBinder
  {
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
      // Check if the model type is enumerable
      if (!bindingContext.ModelMetadata.IsEnumerableType)
      {
        bindingContext.Result = ModelBindingResult.Failed();
        return Task.CompletedTask;
      }

      // Get the value from the request
      var providedValue = bindingContext.ValueProvider
        .GetValue(bindingContext.ModelName)
        .ToString();

      // If no value is provided, set the result to null and return success
      if (string.IsNullOrEmpty(providedValue))
      {
        bindingContext.Result = ModelBindingResult.Success(null);
        return Task.CompletedTask;
      }

      // Get the type of the elements in the array
      var genericType =
        bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

      // Get a converter for the element type
      var converter = TypeDescriptor.GetConverter(genericType);

      // Split the provided value by comma and convert each element to the appropriate type
      var objectArray = providedValue.Split(new[] { "," },
          StringSplitOptions.RemoveEmptyEntries)
        .Select(x => converter.ConvertFromString(x.Trim()))
        .ToArray();

      // Create an array of the appropriate type and copy the elements to it
      var guidArray = Array.CreateInstance(genericType, objectArray.Length);
      objectArray.CopyTo(guidArray, 0);

      // Set the model to the created array
      bindingContext.Model = guidArray;

      // Set the result to the successfully created model
      bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
      return Task.CompletedTask;
    }
  }
}