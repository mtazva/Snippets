/// <reference path="jquery-1.7.2-vsdoc.js" />
/// <reference path="knockout-2.1.0.debug.js"/> 
/// <reference path="moment.js"/>

//add custom binding for converting JSON dates to formatted date string
ko.bindingHandlers.dateString = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor(),
            allBindings = allBindingsAccessor(),
            valueUnwrapped = ko.utils.unwrapObservable(value),
            pattern, dateText;
        if (valueUnwrapped === undefined || valueUnwrapped === null) {
            $(element).text("");
            return;
        }
        if (!valueUnwrapped.getTime) {
            valueUnwrapped = parseInt(valueUnwrapped.substr(6)); // .net format
            if (valueUnwrapped <= 0) {
                // min value
                $(element).text("");
                return;
            }
        }
        else if (valueUnwrapped.getTime() === 0) {
            // empty
            $(element).text("");
            return;
        }
        pattern = allBindings.datePattern || "MM/DD/YYYY";
        dateText = moment(valueUnwrapped).format(pattern);
        $(element).text(dateText);
    }
};

ko.loadTemplateFromString = function (templateName, content) {
    /// <summary>
    /// Loads a knockout template from a string by appending it to the page body.
    /// </summary>
    /// <param name="templateName">Maps to the Knockout TemplateID</param>
    /// <param name="content">The template content</param>
    if (ko.templateIsLoaded(templateName)) {
        // Already exists
        return;
    }
    $("<script id='" + templateName + "' type='text/html'>" + content + "</script>").appendTo("body");
};

ko.loadExternalTemplate = function (url, templateName, successCallback, failCallback) {
    /// <summary>
    /// Loads a knockout template from a URL by creating an AJAX call and then appending it to the page body.
    /// </summary>
    /// <param name="url">The URL where the template content exists</param>
    /// <param name="templateName">Maps to the Knockout TemplateID</param>
    /// <param name="successCallback">(Optional) The callback to fire on success</param>
    /// <param name="failCallback">(Optional) The callback to fire on failure</param>
    ko.loadExternalTemplateWithOptions(url, "GET", templateName, false, successCallback, failCallback, null);
};

ko.loadExternalTemplateWithOptions = function (url, action, templateName, returnsJson, successCallback, failCallback, templateData) {
    /// <summary>
    /// Loads a knockout template from a URL by creating an AJAX call and then appending it to the page body.
    /// </summary>
    /// <param name="url">The URL where the template content exists</param>
    /// <param name="action">The HTTP verb to use with the request</param>
    /// <param name="templateName">Maps to the Knockout TemplateID</param>
    /// <param name="returnsJson">Whether or not the URL will return JSON data</param>
    /// <param name="successCallback">(Optional) The callback to fire on success</param>
    /// <param name="failCallback">(Optional) The callback to fire on failure</param>
    /// <param name="templateData">(Optional) the JSON data to send to the URL</param>
    if (ko.templateIsLoaded(templateName)) {
        // Already exists
        successCallback({ templateName: templateName, url: url });
    }

    var options = { type: action };
    if (templateData !== undefined && templateData !== null) {
        options.contentType = "application/json, charset=utf-8";
        options.dataType = "json";
        options.data = $.toJSON(templateData);
    }
    $.ajax(url, options)
        .done(function (data) {
            if (returnsJson) {
                data = data.Template;
            }
            ko.loadTemplateFromString(templateName, data);
            if (successCallback !== undefined && successCallback !== null) {
                successCallback({ templateName: templateName, url: url });
            }
        })
        .fail(function (data) {
            if (failCallback !== undefined && failCallback !== null) {
                data.templateName = templateName;
                data.url = url;
                failCallback(data);
            }
        });
};

ko.templateIsLoaded = function (templateName) {
    /// <summary>
    /// Returns whether or not a script tag with an id matching the "templateName" already exists on the page
    /// </summary>
    /// <param name="templateName">The template name to check for</param>
    /// <returns type="boolean">Whether or not a script tag with an id matching the "templateName" already exists on the page</returns>
    return ($("script#" + templateName).size() > 0);
};

ko.ExternalTemplateDefinition = function (templateName, action, returnsJson, url, templateData) {
    /// <summary>
    /// Constructor for creating a knockout External Template definition, for use with 
    /// ko.loadExternalTemplateList()
    /// </summary>
    /// <param name="templateName">Maps to the Knockout TemplateID</param>
    /// <param name="action">The HTTP verb to use with the request</param>
    /// <param name="returnsJson">Whether or not the URL will return JSON data</param>
    /// <param name="url">The URL where the template content exists</param>
    /// <param name="templateData">(Optional) the JSON data to send to the URL</param>
    this.TemplateName = templateName;
    this.Action = action;
    this.ReturnsJson = returnsJson;
    this.URL = url;
    this.TemplateData = templateData
};

ko.loadExternalTemplateList = function (templateObjects, successCallback, failCallback) {
    /// <summary>
    /// Loads a list of knockout templates specified by the templateObjects array and fires callbacks
    /// when the batch has completed.
    /// </summary>
    /// <param name="templateObjects">An array of ko.ExternalTemplateDefinition objects</param>
    /// <param name="successCallback">(Optional) The callback to fire on success (all successful)</param>
    /// <param name="failCallback">(Optional) The callback to fire on failure (at least one failure)</param>
    var i,
        templateStatus = [],
        missingTemplates = [];
    if (templateObjects === null) {
        return; // No templates passed in
    }

    // Add templates that have not already been loaded
    for (i = 0; i < templateObjects.length; i++) {
        if (!ko.templateIsLoaded(templateObjects[i].TemplateName)) {
            missingTemplates[missingTemplates.length] = templateObjects[i];
        }
    }

    if (missingTemplates === null || missingTemplates.length === 0) {
        successCallback();
        return; // Nothing to do
    }

    // Define polling function
    var pollForTemplateLoad = function () {
        var j;
        for (j = 0; j < templateStatus.length; j++) {
            if (templateStatus[j] === null) {
                window.setTimeout(pollForTemplateLoad, 50);
                return;
            }
        }

        // All loaded
        var failedTemplates = [];
        for (j = 0; j < templateStatus.length; j++) {
            if (templateStatus[j] !== true) {
                failedTemplates[failedTemplates.length] = templateStatus[j];
            }
        }

        // Fire callbacks
        if (failedTemplates.length === 0) {
            if (successCallback !== undefined && successCallback !== null) {
                successCallback();
            }
        }
        else {
            if (failCallback !== undefined && failCallback !== null) {
                failCallback({ FailedTemplates: failedTemplates });
            }
        }
    };

    // Define function to update status based on templateName
    var updateEntryStatus = function (templateName, status) {
        var j;
        for (j = 0; j < missingTemplates.length; j++) {
            if (missingTemplates[j].TemplateName === templateName) {
                templateStatus[j] = status;
                return;
            }
        }
    };

    // Load all requested templates
    for (i = 0; i < missingTemplates.length; i++) {
        templateStatus[i] = null;
        ko.loadExternalTemplateWithOptions(
            missingTemplates[i].URL,
            missingTemplates[i].Action,
            missingTemplates[i].TemplateName,
            missingTemplates[i].ReturnsJson,
            function (data) {
                updateEntryStatus(data.templateName, true);
            },
            function (data) {
                updateEntryStatus(data.templateName, { TemplateName: data.templateName, URL: data.url, ErrorData: data });
            },
            missingTemplates[i].TemplateData);
    }

    // Start polling
    pollForTemplateLoad();
};

ko.dirtyFlag = function (root, isInitiallyDirty) {
    /// <summary>
    /// Creates a dirty flag object used for change detection
    /// <para>
    /// Usage Example: this.dirtyFlag = new ko.dirtyFlag(this);
    /// </para><para>
    /// Reference URL: http://www.knockmeout.net/2011/05/creating-smart-dirty-flag-in-knockoutjs.html
    /// </para>
    /// </summary>
    /// <param name="root">The object to detect changes against</param>
    /// <param name="isInitiallyDirty">Whether or not the object is initially dirty (e.g. new object)</param>
    /// <returns type="function"></returns>
    var result = function () { },
        _initialState = ko.observable(ko.toJSON(root)),
        _isInitiallyDirty = ko.observable(isInitiallyDirty);

    result.isDirty = ko.computed(function () {
        // For debugging purposes since Chrome currently has a bug expanding watch window results...
        //        var origItem = _initialState();
        //        var newItem = ko.toJSON(root);
        return _isInitiallyDirty() || _initialState() !== ko.toJSON(root);
    });

    result.reset = function () {
        _initialState(ko.toJSON(root));
        _isInitiallyDirty(false);
        result.isDirty.notifySubscribers(false);
    };

    return result;
};

ko.flatten = function (json) {
    /// <summary>
    /// Flattens a JSON object into a one-level object. For example, { one: { two: 'three'; } } would become { 'one.two': 'three' }.
    /// </summary>
    /// <param name="json">The json object to flatten</param>
    /// <returns type="object">The flattened object</returns>
    var nj = {},
        walk = function (p, j) {
            var jp, prop, k;
            for (prop in j) {
                jp = j[prop];
                if (jp.toString() === "[object Object]") {
                    if (p !== null) { k = p + "." + prop; }
                    else { k = prop; }
                    walk(k, jp);
                }
                else {
                    if (p !== null) { k = p + "." + prop; }
                    else { k = prop; }
                    nj[k] = jp;
                }
            }
        };
    walk(null, json);
    return nj;
};