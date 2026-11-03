let lineChart;

const axisColor = "#38383b";
const tickColor = "rgba(255, 255, 255, 0.45)";
// Per-chart hover titles kept out of the chart config so updates can swap them freely.
const chartTitles = new WeakMap();

window.drawLineGraph = (labels, data, titles) => {
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
						// Titles come from the server (full date per point); fall back to the axis label.
						title: (items) => {
							const t = chartTitles.get(items[0].chart);
							return (t && t[items[0].dataIndex]) || items[0].label;
						},
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
						// Month/day labels repeat every year, so mark the first tick and each
						// January with the year to keep a multi-year span readable.
						callback: function (value, index) {
							const label = this.getLabelForValue(value);
							if (!/^[A-Z][a-z]{2} \d+$/.test(label)) return label;
							const year = chartTitles.get(lineChart)?.[value]?.match(/\d{4}/)?.[0];
							if (!year) return label;
							return index === 0 || label.startsWith("Jan") ? `${label}, ${year}` : label;
						},
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
	chartTitles.set(lineChart, titles);
};

window.updateLineGraph = (labels, data, titles) => {
	if (lineChart) {
		lineChart.data.labels = labels;
		lineChart.data.datasets[0].data = data;
		chartTitles.set(lineChart, titles);
		lineChart.update();
	}
};
