$(document).ready(function () {

  "use strict";

  // ==================================================
  // Country selection tree management
  // ==================================================

  // Show / hide subordinate branch of tree depending on state of toggle checkbox.
  //  node - Checkbox input element (hidden) controlling expand/collapse of associated tree branch.
  function treeToggle(node) {
    if (node.prop("checked")) {
      node.parent().removeClass("collapsed").siblings("ul").show();
    } else {
      node.parent().addClass("collapsed").siblings("ul").hide();
    }
  }

  // Set state of designated tree node based on the state of its descendants.
  //  node - li element containing checkbox whose state is being set.
  function setTreeCheckbox(node) {
    var currentCheckbox = $(node).find("> .select > input");
    var descendentLeafNodes = $(node).find("li").filter(function () {
      return $(this).children("ul").length == 0;
    }).find(".select > input");
    if (descendentLeafNodes.length > 0) {
      switch (descendentLeafNodes.filter(".select > input:checked").length) {
      case 0:
        currentCheckbox.prop("checked", false).prop("indeterminate", false);
        break;
      case descendentLeafNodes.length:
        currentCheckbox.prop("checked", true).prop("indeterminate", false);
        break;
      default:
        currentCheckbox.prop("checked", true).prop("indeterminate", true);
      }
    }
  }

  // Set click event handler to collapse/expand selection tree and to manage the state of
  //  parent and child node checkboxes.
  // Set initial collapse/expand state and inital state of selection checkboxes.
  $(".tree-select").click(function (event) {
    var node = $(event.target);
    if (node.is(".toggle input")) {
      treeToggle(node);
    } else if (node.is(".select input")) {
      // Check/uncheck all child checkboxes
      var currentNode = node.parent().parent();
      var descendantNodes = currentNode.find("li .select > input");
      descendantNodes.prop("checked", node.prop("checked")).prop("indeterminate", false);

      // Navigate to successive parent nodes, setting their checkboxes based on the combined
      //  state of their child checkboxes
      currentNode.parents(".tree-select li").each(function () {
        setTreeCheckbox(this);
      });
    }
  }).find("li").each(function () {
    setTreeCheckbox(this);
  }).end().find(".toggle input").each(function () {
    treeToggle($(this));
  });

  // ==================================================
  // Filtered country selection list management
  // ==================================================

  // Display countries filtered by search box text.
  //  searchBox - Text input element acting as search box for associated list.
  function filterList(searchBox) {
    var filter = searchBox.val();
    var countryList = searchBox.parent().next().find("li");
    if (filter) {
      $(countryList).each(function () {
        if ($(this).find("label").text().match(RegExp(filter, 'i'))) {
          $(this).show();
        } else {
          $(this).hide();
        }
      });
    } else {
      $(countryList).show();
    }
  }

  // Set state of "Select all" checkbox depending on the state of the visible checkboxes in
  //  the filtered selection list.
  //  selectList - div element (class: list-select) encapsulating the filtered selection list.
  function setSelectAll (selectList) {
    var selectAllCheckbox = selectList.siblings("label").children("input");
    var visibleCheckboxes = selectList.find("li:visible .select > input");
    if (visibleCheckboxes.length > 0) {
      switch (visibleCheckboxes.filter(":checked").length) {
      case 0:
        selectAllCheckbox.prop("checked", false).prop("indeterminate", false);
        break;
      case visibleCheckboxes.length:
        selectAllCheckbox.prop("checked", true).prop("indeterminate", false);
        break;
      default:
        selectAllCheckbox.prop("checked", true).prop("indeterminate", true);
      }
    }
  }

  // Set initially displayed countries and restrict countries to those whose names match entered
  //  filter string.
  $(".search input").each(function () {
    var searchBox = $(this);
    filterList(searchBox);
    setSelectAll(searchBox.parent().next().find(".list-select"));
  }).bind("change keyup", function () {
    var searchBox = $(this);
    filterList(searchBox);
    setSelectAll(searchBox.parent().next().find(".list-select"));
  });

  // Clear search box when "clear" icon is clicked.
  $(".search img").click(function () {
    var searchBox = $(this).siblings("input");
    searchBox.val("");
    filterList(searchBox);
    setSelectAll(searchBox.parent().next().find(".list-select"));
  });

  // Check/uncheck all visible country selection checkboxes when "Select all" checkbox is clicked.
  $(".selection-panel > label input").click(function () {
    var checked = $(this).prop("checked");
    $(this).parent().next().find("li:visible .select input").prop("checked", checked);
  });

  // Set initial state of "Select all" checkbox and adjust it whenever one of the visible checkboxes
  //  is clicked.
  $(".list-select").each(function () {
    setSelectAll($(this));
  }).click(function (event) {
    if ($(event.target).is(".select input")) {
      setSelectAll($(this));
    }
  });

  // Ensure at least one population type is selected
  var popTypes = $(".population-types input");
  popTypes.click(function () {
    if (popTypes.filter(":checked").length == 0) {
      $(".population-types input[value='TPOC']").prop("checked", true);
    }
  });

  // // Open new page on country selection in Frequently Requested Statistics
  // $(".frs .residing select").change(function (event) {
  //   if (event.target.value !== "0") {
  //     window.open("PSQ_TMS.aspx?SYR=2000&EYR=2012&RES=" + event.target.value + "&POPT=RF&DRES=N&DPOPT=N");
  //   }
  // });
  // $(".frs .origin select").change(function (event) {
  //   if (event.target.value !== "0") {
  //     window.open("PSQ_TMS.aspx?SYR=2000&EYR=2012&OGN=" + event.target.value + "&POPT=RF&DOGN=N&DPOPT=N");
  //   }
  // });

});
