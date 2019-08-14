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
};

function changetodate() {
    var e = document.getElementById("from_d");
    console.log(e.value);
    var e_tochange = document.getElementById("to_d")
    document.getElementById("to_tr").style.display = "block";
    e_tochange.min = e.value;
};

function maxfromdate() {
    var e = document.getElementById("to_d");
    console.log(e.value);
    var e_tochange = document.getElementById("from_d")
    e_tochange.max = e.value;
};

