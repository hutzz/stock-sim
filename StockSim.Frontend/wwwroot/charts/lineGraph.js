let lineChart;

const axisColor = "#38383b";
const tickColor = "rgba(255, 255, 255, 0.45)";

window.drawLineGraph = (labels, data) => {
	const ctx = document.getElementById("lineChart");
	const chartData = {
		labels: labels,
		datasets: [
			{
				label: "Value",
				data: data,
				fill: false,
				borderColor: "#4b73e3",
				backgroundColor: "rgba(75, 115, 227, 0.12)",
				pointBackgroundColor: "#b5c5eb",
				pointBorderColor: "#b5c5eb",
				pointRadius: 4,
				pointHoverRadius: 6,
				tension: 0.2,
			},
		],
	};
	const config = {
		type: "line",
		data: chartData,
		options: {
			responsive: true,
			maintainAspectRatio: false,
			plugins: {
				legend: {
					display: false,
				},
				tooltip: {
					backgroundColor: "rgb(36, 34, 34)",
					borderColor: "rgba(75, 115, 227, 0.45)",
					borderWidth: 1,
					titleColor: "white",
					bodyColor: "rgba(255, 255, 255, 0.8)",
					padding: 10,
					cornerRadius: 8,
					displayColors: false,
					callbacks: {
						label: (item) => `$${Number(item.parsed.y).toFixed(2)}`,
					},
				},
			},
			scales: {
				x: {
					grid: {
						color: axisColor,
					},
					ticks: {
						color: tickColor,
						font: { size: 11 },
						maxRotation: 0,
						autoSkip: true,
						maxTicksLimit: 8,
					},
				},
				y: {
					grid: {
						color: axisColor,
					},
					ticks: {
						color: tickColor,
						font: { size: 11 },
						callback: (value) => `$${Number(value).toFixed(2)}`,
					},
				},
			},
		},
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
