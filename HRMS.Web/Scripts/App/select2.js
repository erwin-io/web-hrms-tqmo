$(document).ready(function() {
		// Simple Selectbox
		$(".select-simple").select2({
			theme: "bootstrap",
			minimumResultsForSearch: Infinity,
		});
		// Selectbox with search
		$(".select-with-search").select2({
			theme: "bootstrap"
		});
		// Select Multiple Tags
		$(".select-tags").select2({
			tags: false,
			theme: "bootstrap",
		});
		// Select & Add Multiple Tags
		$(".select-add-tags").select2({
			tags: true,
			theme: "bootstrap",
		});
	});