let lineChart;

window.drawLineGraph = (labels, data) => {
	const ctx = document.getElementById("lineChart");
	const chartData = {
		labels: labels,
		datasets: [
			{
				label: "Value",
				data: data,
				fill: false,
				borderColor: "#32456b",
				backgroundColor: "#32456b",
				pointBackgroundColor: "#b5c5eb",
				pointBorderColor: "#b5c5eb",
				pointRadius: 5,
				tension: 0.2,
			},
		],
	};
	const config = {
		type: "line",
		data: chartData,
	};
	lineChart = new Chart(ctx, config);
};

window.updateLineGraph = (labels, data) => {
	if (lineChart) {
		lineChart.data.labels = labels;
		lineChart.data.datasets[0].data = data;
		lineChart.update();
	}
};
