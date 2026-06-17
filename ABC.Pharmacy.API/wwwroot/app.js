const API = '/api/medicines';

// Load list on page load
window.onload = function () {
	loadMedicines();
};

// Fetch all medicines and render table
async function loadMedicines() {
	const res  = await fetch(API + '/all');
	const list = await res.json();
	renderTable(list);
}

// Search by name
async function searchMedicines() {
	const term = document.getElementById('searchInput').value.trim();
	if (term === '') {
		loadMedicines();
		return;
	}
	const res  = await fetch(API + '/search?term=' + encodeURIComponent(term));
	const list = await res.json();
	renderTable(list);
}

// Build table rows
function renderTable(list) {
	const tbody  = document.getElementById('medicineTableBody');
	const today  = new Date();
	today.setHours(0, 0, 0, 0);
	const in30   = new Date(today);
	in30.setDate(today.getDate() + 30);

	if (!list || list.length === 0) {
		tbody.innerHTML = '<tr><td colspan="7">No medicines found.</td></tr>';
		return;
	}

	tbody.innerHTML = '';
	list.forEach(function (med, i) {
		const expiry     = new Date(med.expiryDate);
		const expirySoon = expiry <= in30;
		const lowStock   = med.quantity < 10;
		const rowClass   = expirySoon ? 'row-red' : (lowStock ? 'row-yellow' : '');

		const tr     = document.createElement('tr');
		tr.className = rowClass;
		tr.innerHTML =
			'<td>' + (i + 1) + '</td>' +
			'<td>' + med.fullName + '</td>' +
			'<td>' + med.brand + '</td>' +
			'<td>$' + Number(med.price).toFixed(2) + '</td>' +
			'<td>' + med.quantity + '</td>' +
			'<td>' + expiry.toLocaleDateString() + '</td>' +
			'<td><button onclick="sellMedicine(\'' + med.id + '\', \'' + med.fullName + '\')">Sell</button></td>';
		tbody.appendChild(tr);
	});
}

// Add a new medicine
async function addMedicine(e) {
	e.preventDefault();
	const payload = {
		fullName:   document.getElementById('add_fullName').value,
		brand:      document.getElementById('add_brand').value,
		price:      parseFloat(document.getElementById('add_price').value),
		quantity:   parseInt(document.getElementById('add_quantity').value),
		expiryDate: document.getElementById('add_expiryDate').value,
		notes:      document.getElementById('add_notes').value
	};
	const res = await fetch(API + '/add', {
		method:  'POST',
		headers: { 'Content-Type': 'application/json' },
		body:    JSON.stringify(payload)
	});
	if (res.ok) {
		document.getElementById('msg').textContent = 'Medicine added successfully.';
		e.target.reset();
		loadMedicines();
	} else {
		document.getElementById('msg').style.color = 'red';
		document.getElementById('msg').textContent = 'Failed to add medicine.';
	}
}

// Sell – ask quantity via browser prompt
async function sellMedicine(id, name) {
	const qty = prompt('Enter quantity to sell for: ' + name);
	if (qty === null || qty === '') return;
	const res = await fetch(API + '/sell', {
		method:  'POST',
		headers: { 'Content-Type': 'application/json' },
		body:    JSON.stringify({ medicineId: id, quantity: parseInt(qty) })
	});
	if (res.ok) {
		loadMedicines();
	} else {
		alert('Sale failed. Please try again.');
	}
}
