document.addEventListener("DOMContentLoaded", () => {
	const modalElement = document.getElementById("crud-modal");
	if (!modalElement || typeof bootstrap === "undefined") {
		return;
	}

	const modalBody = modalElement.querySelector("[data-modal-body]");
	const modalTitle = document.getElementById("crud-modal-title");
	const modal = new bootstrap.Modal(modalElement);
	const loadingMarkup = `
		<div class="location-modal-loading text-center py-4">
			<div class="spinner-border text-primary" role="status" aria-hidden="true"></div>
			<p class="text-muted mt-3 mb-0">Cargando formulario…</p>
		</div>`;

	let lastTrigger = null;
	let refreshUrl = "";
	let refreshTargetSelector = "";

	function parseValidation() {
		if (!window.jQuery || !jQuery.validator || !jQuery.validator.unobtrusive) {
			return;
		}

		const form = modalBody.querySelector("form[data-modal-form]");
		if (!form) {
			return;
		}

		jQuery(form).removeData("validator");
		jQuery(form).removeData("unobtrusiveValidation");
		jQuery.validator.unobtrusive.parse(form);
	}

	function focusFirstField() {
		const focusable = modalBody.querySelector("input, select, textarea, button");
		if (focusable) {
			focusable.focus();
		}
	}

	function renderError(message) {
		modalBody.innerHTML = `<div class="alert alert-danger mb-0" role="alert">${message}</div>`;
	}

	async function refreshList() {
		const target = document.querySelector(refreshTargetSelector);
		if (!refreshUrl || !target) {
			window.location.reload();
			return;
		}

		const response = await fetch(refreshUrl, {
			headers: {
				"X-Requested-With": "XMLHttpRequest"
			}
		});

		if (!response.ok) {
			window.location.reload();
			return;
		}

		target.innerHTML = await response.text();
	}

	async function loadModal(trigger) {
		lastTrigger = trigger;
		refreshUrl = trigger.dataset.modalRefreshUrl || "";
		refreshTargetSelector = trigger.dataset.modalRefreshTarget || "";
		modalTitle.textContent = trigger.dataset.modalTitle || "Gestionar elemento";
		modalBody.innerHTML = loadingMarkup;
		modal.show();

		const response = await fetch(trigger.dataset.modalUrl, {
			headers: {
				"X-Requested-With": "XMLHttpRequest"
			}
		});

		if (!response.ok) {
			renderError("No se pudo cargar el formulario solicitado.");
			return;
		}

		modalBody.innerHTML = await response.text();
		parseValidation();
		focusFirstField();
	}

	document.addEventListener("click", async event => {
		const trigger = event.target.closest("[data-modal-url]");
		if (!trigger) {
			return;
		}

		event.preventDefault();
		await loadModal(trigger);
	});

	modalElement.addEventListener("submit", async event => {
		const form = event.target.closest("form[data-modal-form]");
		if (!form) {
			return;
		}

		event.preventDefault();

		const response = await fetch(form.action || window.location.href, {
			method: form.method || "post",
			body: new FormData(form),
			headers: {
				"X-Requested-With": "XMLHttpRequest"
			}
		});

		if (response.status === 204) {
			modal.hide();
			await refreshList();
			return;
		}

		if (response.ok || response.status === 400) {
			modalBody.innerHTML = await response.text();
			parseValidation();
			focusFirstField();
			return;
		}

		renderError("No se pudo completar la operación.");
	});

	modalElement.addEventListener("hidden.bs.modal", () => {
		modalBody.innerHTML = loadingMarkup;
		if (lastTrigger instanceof HTMLElement) {
			lastTrigger.focus();
		}
	});
});
