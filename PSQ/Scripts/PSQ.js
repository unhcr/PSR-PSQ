$(document).ready(function () {

  "use strict";

  // Ensure valid date range
  $("select.start-year").click(function (event) {
    $("select.end-year option").each(function () {
      $(this).prop("disabled", (this.value < event.target.value));
    });
  });
  $("select.end-year").click(function (event) {
    $("select.start-year option").each(function () {
      $(this).prop("disabled", (this.value > event.target.value));
    });
  });
  var selectedStart = $("select.start-year option:selected").val();
  $("select.end-year option").each(function () {
    $(this).prop("disabled", (this.value < selectedStart));
  });

  // Manage "All ..." selections in country and origin selects
  $("select.country").click(function (event) {
    if (event.target.value === "0") {
      $("select.country option[value!='0']").each(function () {
        $(this).prop("selected", false);
      })
    } else {
      $("select.country option[value='0']").prop("selected", false);
    }
  });
  $("select.origin").click(function (event) {
    if (event.target.value === "0") {
      $("select.origin option[value!='0']").each(function () {
        $(this).prop("selected", false);
      })
    } else {
      $("select.origin option[value='0']").prop("selected", false);
    }
  });

  // Open new page on country selection in Frequently Requested Statistics
  $(".frs .residing select").change(function (event) {
    if (event.target.value !== "0") {
      window.open("PSQ_TMS.aspx?SYR=2000&EYR=2012&RES=" + event.target.value + "&POPT=RF&DRES=N&DPOPT=N")
    }
  });
  $(".frs .origin select").change(function (event) {
    if (event.target.value !== "0") {
      window.open("PSQ_TMS.aspx?SYR=2000&EYR=2012&OGN=" + event.target.value + "&POPT=RF&DOGN=N&DPOPT=N")
    }
  });

  // Ensure at least one numeric value column is selected
  var popTypes = $(".values input");
  popTypes.click(function () {
    if (popTypes.filter(":checked").length == 0) {
      $("#BodyPlaceHolder_cbxPOC").prop("checked", true);
    }
  })

});