# Cascading DropDownList Helper

## The usage:

    @using Mvc.CascadeDropDown

    //First simple dropdown 
    @Html.DropDownListFor(m=>m.SelectedCountry, Model.Countries,
    "Please select a Country", new {@class="form-control"})

    //Dropdown list for SelectedCity property that depends on selection of SelectedCountry property
    @Html.CascadingDropDownListFor( 
      expression: m => m.SelectedCity, 
      triggeredByProperty: m => m.SelectedCountry,  //Parent property that trigers dropdown data loading
      url: Url.Action("GetCities", "Home"),  //Url of action that returns dropdown data
      ajaxActionParamName: "country",   //Parameter name for the selected parent value that url action receives
      optionLabel: "Please select a City", // Option label
      disabledWhenParrentNotSelected: true, //If true, disables dropdown until parrent dropdown selected
      htmlAttributes: new { @class = "form-control" }) //Html attributes

    //Dropdown list for SelectedStreet property that depends on selection of SelectedCity property
    @Html.CascadingDropDownListFor(m => m.SelectedStreet, m => m.SelectedCity, 
    Url.Action("GetStreets", "Home"), "city", "Please select a Street", true, new { @class = "form-control" })

## Versions:

### v1.3.2:

* Added support for disabled options

### v2.0
* **Breaking change** Fixed typo in `disabledWhenParrentNotSelected` argument name - fixed name is 
 `disabledWhenParentNotSelected`
* Introduced CascadeDropDownOptions that define the behavior of the helper and allow to set callbacks for an events which happen in casccading dropdown flow such as BeforeSend, OnCompleteGetData, OnFailureGetData, OnSuccessGetData.

### v2.0.1
* Fix for dropdown id resolving when using nested models (https://github.com/alexanderar/Mvc.CascadeDropDown/issues/26)
