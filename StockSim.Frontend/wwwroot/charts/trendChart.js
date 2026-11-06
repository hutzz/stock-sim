// Portfolio trend over time from stored daily snapshots. Points are always visible so a
// week of history reads as data, not as an empty plot.
let trendChart;

window.drawPortfolioTrend = (labels, totals) => {
	const ctx = document.getElementById("portfolioTrend");
	if (!ctx) return;
	trendChart = new Chart(ctx, {
		type: "line",
		data: {
			labels: labels,
			datasets: [
				{
					label: "Total account value",
					data: totals,
					fill: true,
					borderColor: CHART_ACCENT,
					backgroundColor: window.withAlpha("#4b73e3", 0.14),
					pointBackgroundColor: "#b5c5eb",
					pointBorderColor: "#b5c5eb",
					pointRadius: 4,
					pointHoverRadius: 6,
					tension: 0.2,
				},
			],
		},
		options: {
			responsive: true,
			maintainAspectRatio: false,
			plugins: {
				legend: { display: false },
				tooltip: {
					...CHART_TOOLTIP,
					displayColors: false,
					callbacks: {
						label: (item) => `$${Number(item.parsed.y).toFixed(2)}`,
					},
				},
			},
			scales: {
				x: {
					grid: { color: CHART_AXIS_COLOR },
					ticks: {
						color: CHART_TICK_COLOR,
						font: { size: 11 },
						maxRotation: 0,
						autoSkip: true,
						maxTicksLimit: 8,
					},
				},
				y: {
					grid: { color: CHART_AXIS_COLOR },
					ticks: {
						color: CHART_TICK_COLOR,
						font: { size: 11 },
						callback: (value) => `$${Number(value).toFixed(2)}`,
					},
				},
			},
		},
	});
};

window.updatePortfolioTrend = (labels, totals) => {
	if (!trendChart) return;
	trendChart.data.labels = labels;
	trendChart.data.datasets[0].data = totals;
	trendChart.update();
};
