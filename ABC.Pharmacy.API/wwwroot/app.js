const API = '/api/medicines';

// ── Notification toast ───────────────────────────────────────────────────────
let _toastTimer = null;

function showNotification(message, type /* 'success' | 'error' */ = 'error') {
	const toast = document.getElementById('toast');
	toast.textContent = message;
	toast.className   = 'show toast-' + type;

	if (_toastTimer) clearTimeout(_toastTimer);
	_toastTimer = setTimeout(function () {
		toast.className = '';
	}, 4000);
}

// Extract a human-readable error message from a failed Response
async function getErrorMessage(res) {
	try {
		const data = await res.json();
		return data.error || data.title || JSON.stringify(data);
	} catch {
		return 'Request failed (' + res.status + ' ' + res.statusText + ')';
	}
}

// ── Load list on page load ────────────────────────────────────────────────────
window.onload = function () {
	loadMedicines();
};

// Fetch all medicines and render table
async function loadMedicines() {
	try {
		const res  = await fetch(API + '/all');
		if (!res.ok) {
			const msg = await getErrorMessage(res);
			showNotification('Failed to load medicines: ' + msg);
			return;
		}
		const list = await res.json();
		renderTable(list);
	} catch (err) {
		showNotification('Network error while loading medicines. Please check your connection.');
		console.error(err);
	}
}

// Search by name
async function searchMedicines() {
	const term = document.getElementById('searchInput').value.trim();
	if (term === '') {
		loadMedicines();
		return;
	}
	try {
		const res  = await fetch(API + '/search?term=' + encodeURIComponent(term));
		if (!res.ok) {
			const msg = await getErrorMessage(res);
			showNotification('Search failed: ' + msg);
			return;
		}
		const list = await res.json();
		renderTable(list);
	} catch (err) {
		showNotification('Network error while searching. Please check your connection.');
		console.error(err);
	}
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
	try {
		const res = await fetch(API + '/add', {
			method:  'POST',
			headers: { 'Content-Type': 'application/json' },
			body:    JSON.stringify(payload)
		});
		if (res.ok) {
			showNotification('Medicine added successfully.', 'success');
			document.getElementById('msg').textContent = '';
			e.target.reset();
			loadMedicines();
		} else {
			const msg = await getErrorMessage(res);
			showNotification('Failed to add medicine: ' + msg);
		}
	} catch (err) {
		showNotification('Network error while adding medicine. Please check your connection.');
		console.error(err);
	}
}

// Sell – ask quantity via browser prompt
async function sellMedicine(id, name) {
	const qty = prompt('Enter quantity to sell for: ' + name);
	if (qty === null || qty === '') return;

	const quantity = parseInt(qty);
	if (isNaN(quantity) || quantity <= 0) {
		showNotification('Please enter a valid positive quantity.');
		return;
	}

	try {
		const res = await fetch(API + '/sell', {
			method:  'POST',
			headers: { 'Content-Type': 'application/json' },
			body:    JSON.stringify({ medicineId: id, quantity: quantity })
		});
		if (res.ok) {
			showNotification('Sale recorded successfully.', 'success');
			loadMedicines();
		} else {
			const msg = await getErrorMessage(res);
			showNotification('Sale failed: ' + msg);
		}
	} catch (err) {
		showNotification('Network error while processing sale. Please check your connection.');
		console.error(err);
	}
}
