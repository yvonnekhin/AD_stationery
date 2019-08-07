function onChange() {
    var e = document.getElementById("itemId");
    console.log(e.options[e.selectedIndex].value);
    var etochange = document.getElementById("itemCode");
    etochange.value = e.options[e.selectedIndex].value;
};

function onChange_rep() {
    var e = document.getElementById("itemId_rep");
    console.log(e.options[e.selectedIndex].value);
    var etochange = document.getElementById("rep_id");
    etochange.value = e.options[e.selectedIndex].value;
};

function onChange_delegate() {
    var e = document.getElementById("delegate_id");
    console.log(e.options[e.selectedIndex].value);
    document.getElementById("delegate_id_hidden").value = e.options[e.selectedIndex].value;
    
    //if (e.options[e.selectedIndex].value == document.getElementById("hidden_dept_head").value) {
    //    document.getElementById("show_hide").style.display = "none";
    //} else {
    //    document.getElementById("show_hide").style.display = "block";
    //}
};

